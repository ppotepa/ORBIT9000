using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Engine.Loaders.Plugin.Details;
using ORBIT9000.Engine.Loaders.Plugin.Results;
using ORBIT9000.Engine.Loaders.Plugin.Validation;
using System.Reflection;

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

        private (Assembly? Assembly, bool ContainsPlugins, Type[] plugins) TryLoadAssembly(string path, List<Exception> exceptions)
        {
            var info = new FileInfo(path);
            Assembly? assembly = null;
            bool containsPlugins = false;
            Type[] pluginTypes = Array.Empty<Type>();

            try
            {
                byte[] assemblyBytes = File.ReadAllBytes(path);
                assembly = Assembly.Load(assemblyBytes);
                pluginTypes = assembly.GetTypes()
                    .Where(type => type.IsClass && typeof(IOrbitPlugin).IsAssignableFrom(type))
                    .ToArray();

                containsPlugins = pluginTypes.Any();

                if (!containsPlugins)
                {
                    _logger?.LogWarning("File does not contain any plugins. {FileName}", info.Name);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Failed to load assembly from {Path}", path);
                exceptions.Add(ex);
            }

            return (assembly, containsPlugins, pluginTypes);
        }

        private PluginLoadDetails TryLoadSingleFile(string path)
        {
            List<Exception> exceptions = new List<Exception>();
            Type[] plugins = Array.Empty<Type>();
            var validator = new PluginFileValidator(path, _logger);

            // Validate the file Path and type
            validator.Validate(exceptions);

            Assembly? loadedAssembly = null;
            bool containsPlugins = false;

            // Only attempt to load if basic validation passes
            if (validator.IsValid)
            {
                (loadedAssembly, containsPlugins, plugins) = TryLoadAssembly(path, exceptions);
            }

            // Handle Error if any occurred
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
