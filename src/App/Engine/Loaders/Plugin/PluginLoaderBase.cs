using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Loaders.Assembly;
using ORBIT9000.Engine.Loaders.Plugin.Details;
using ORBIT9000.Engine.Loaders.Plugin.Results;
using ORBIT9000.Engine.Loaders.Plugin.Validation;
using SystemAssembly = System.Reflection.Assembly;


namespace ORBIT9000.Engine.Loaders.Plugin
{
    internal abstract class PluginLoaderBase<TSource>
    {

        protected readonly ILogger? _logger;
        protected bool _abortOnError = false;

        protected PluginLoaderBase(ILogger? logger = default)
        {
            _logger = logger;
        }

        public PluginLoaderBase<TSource> AbortOnError(bool abortOnError = false)
        {
            _abortOnError = abortOnError;
            return this;
        }
        public abstract IEnumerable<PluginLoadResult> LoadPlugins(TSource source);
        protected PluginLoadResult LoadSingle(string path)
        {
            using (_logger?.BeginScope($"{new FileInfo(path).Name}"))
            {
                _logger?.LogInformation("Loading plugin from {Path}", path);

                PluginLoadDetails details = TryLoadSingleFile(path);

                return new PluginLoadResult(
                    path,
                    details.FileExists,
                    details.IsDll,
                    details.ContainsPlugins,
                    [details.Error],
                    details.LoadedAssembly,
                    details.Plugins
                );
            }
        }

        private static string FormatErrorMessages(List<Exception> exceptions)
            => string.Join('\n', exceptions.Select(ex => ex.Message));

        private void HandleErrors(List<Exception> exceptions)
        {
            if (_abortOnError)
            {
                _logger?.LogCritical("Aborting due to errors:");

                foreach (Exception ex in exceptions)
                {
                    _logger?.LogCritical(ex, ex.Message);
                }

                switch (exceptions.Count)
                {
                    case 1:
                        throw exceptions[0];
                    default:
                        throw new AggregateException("Multiple exceptions occurred", exceptions);
                }
            }
        }

        private PluginLoadDetails TryLoadSingleFile(string path)
        {
            List<Exception> exceptions = new List<Exception>();
            Type[] plugins = Array.Empty<Type>();
            var validator = new PluginFileValidator(path, _logger);
            
            validator.Validate(exceptions);

            SystemAssembly? loadedAssembly = null;
            bool containsPlugins = false;
            
            if (validator.IsValid)
            {
                (loadedAssembly, containsPlugins, plugins) = AssemblyLoader.TryLoadAssembly(path, exceptions);
            }
            
            if (exceptions.Count != 0)
            {
                loadedAssembly = null;
                containsPlugins = false;

                HandleErrors(exceptions);
            }

            return new PluginLoadDetails(
                FileExists: validator.FileExists,
                IsDll: validator.IsDll,
                ContainsPlugins: containsPlugins,
                Error: FormatErrorMessages(exceptions),
                LoadedAssembly: loadedAssembly,
                Plugins: plugins
            );
        }
    }
}
