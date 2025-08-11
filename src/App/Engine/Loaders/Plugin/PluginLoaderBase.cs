using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Loaders;
using ORBIT9000.Engine.Loaders.Plugin.Details;
using ORBIT9000.Engine.Loaders.Plugin.Results;
using ORBIT9000.Engine.Loaders.Plugin.Validation;

namespace ORBIT9000.Engine.Loaders.Plugin
{
    internal abstract class PluginLoaderBase<TSource> : IPluginLoader<TSource>
        where TSource : class
    {
        protected readonly ILogger? _logger;
        protected bool _abortOnError = false;
        private readonly IAssemblyLoader _assemblyLoader;
        private readonly Configuration.Raw.RawConfiguration _config;

        protected PluginLoaderBase(ILogger? logger, Configuration.Raw.RawConfiguration config, IAssemblyLoader assemblyLoader)
        {
            _logger = logger;
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _assemblyLoader = assemblyLoader;
        }
  
        public abstract IEnumerable<Results.AssemblyLoadResult> LoadPlugins(TSource source);
        public IEnumerable<AssemblyLoadResult> LoadPlugins(object source)
        {
            return LoadPlugins((TSource)source);
        }

        protected Results.AssemblyLoadResult LoadSingle(string path)
        {
            FileInfo fileInfo = new FileInfo(path);

            using (_logger?.BeginScope($"{fileInfo.Name}"))
            {
                _logger?.LogInformation("Loading Assembly from {Path}", fileInfo.Name);

                AssemblyLoadResult details = TryLoadSingleFile(fileInfo);

                return details;
            }
        }

        private static string FormatErrorMessages(List<Exception> exceptions)
            => string.Join('\n', exceptions.Select(ex => ex.Message));

        private AssemblyLoadResult CreateAssemblyLoadResult(
            PluginFileValidator fileValidator,
            TryLoadAssemblyResult? assemblyLoadResult,
            List<Exception> exceptions)
        {
            return new AssemblyLoadResult(
                exists: fileValidator.FileExists,
                isDll: fileValidator.IsDll,
                containsPlugins: assemblyLoadResult?.PluginTypes.Any() ?? false,
                error: FormatErrorMessages(exceptions),
                assembly: assemblyLoadResult?.Assembly,
                pluginTypes: assemblyLoadResult?.PluginTypes ?? Array.Empty<Type>(),
                exceptions: exceptions
            );
        }

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

        private AssemblyLoadResult TryLoadSingleFile(FileInfo info)
        {
            var fileValidator = new PluginFileValidator(info, _logger);
            var validationExceptions = fileValidator.Validate().Exceptions;

            if (!fileValidator.IsValid)
            {
                return CreateAssemblyLoadResult(fileValidator, null, validationExceptions);
            }

            var assemblyLoadResult = _assemblyLoader.TryLoadAssembly(info);
            var allExceptions = validationExceptions
                .Concat(assemblyLoadResult.Exceptions ?? Enumerable.Empty<Exception>())
                .ToList();

            if (!assemblyLoadResult.PluginTypes.Any())
            {
                _assemblyLoader.UnloadAssembly(info);
            }

            if (allExceptions.Any())
            {
                HandleErrors(allExceptions);
            }

            return CreateAssemblyLoadResult(fileValidator, assemblyLoadResult, allExceptions);
        }
    }
}
