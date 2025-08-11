using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Loaders.Assembly;
using ORBIT9000.Engine.Loaders.Plugin.Details;
using ORBIT9000.Engine.Loaders.Plugin.Results;
using ORBIT9000.Engine.Loaders.Plugin.Validation;

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
            FileInfo fileInfo = new FileInfo(path);

            using (_logger?.BeginScope($"{fileInfo.Name}"))
            {
                _logger?.LogInformation("Loading Assembly from {Path}", fileInfo.Name);

                PluginLoadDetails details = TryLoadSingleFile(fileInfo);

                return new PluginLoadResult(
                    path,
                    details.FileExists,
                    details.IsDll,
                    details.ContainsPlugins,
                    new[] { details.Error },
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

                if (exceptions.Count is 1)
                {
                    throw exceptions[0];
                }
                if (exceptions.Count > 1)
                {
                    throw new AggregateException("Multiple errors occurred while loading plugins.", exceptions);
                }
            }
        }

        private PluginLoadDetails TryLoadSingleFile(FileInfo info)
        {
            PluginFileValidator fileValidator = new PluginFileValidator(info, _logger);
            List<Exception> validationExceptions = fileValidator.Validate().Exceptions;

            AssemblyLoadResult? assemblyLoadResult = null;

            if (fileValidator.IsValid)
            {
                assemblyLoadResult = AssemblyLoader.TryLoadAssembly(info);
            }

            List<Exception> allExceptions = validationExceptions
                .Concat(assemblyLoadResult?.Exceptions ?? Enumerable.Empty<Exception>())
                .ToList();

            if (allExceptions.Count != 0)
            {
                HandleErrors(allExceptions);
            }

            return new PluginLoadDetails(
                FileExists: fileValidator.FileExists,
                IsDll: fileValidator.IsDll,
                ContainsPlugins: assemblyLoadResult?.ContainsPlugins ?? false,
                Error: FormatErrorMessages(allExceptions),
                LoadedAssembly: assemblyLoadResult?.LoadedAssembly,
                Plugins: assemblyLoadResult?.Plugins ?? Array.Empty<Type>()
            );
        }
    }
}
