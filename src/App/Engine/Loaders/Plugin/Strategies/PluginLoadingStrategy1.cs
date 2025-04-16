using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Plugin;
using ORBIT9000.Engine.Loaders.Plugin.Details;
using ORBIT9000.Engine.Loaders.Plugin.Results;
using ORBIT9000.Engine.Loaders.Plugin.Validation;
using System.Reflection;

namespace ORBIT9000.Engine.Loaders.Plugin.Strategies.ORBIT9000.Engine.Loaders.Plugin.Strategies
{
    internal abstract class PluginLoadingStrategy<TSource> : IPluginLoadingStrategy<TSource>
    {
        private readonly ILogger? _logger = default;

        public PluginLoadingStrategy(ILogger? logger)
        {
            _logger = logger;
        }

        public abstract IEnumerable<PluginLoadResult> LoadPlugins(TSource source);

        public PluginLoadResult LoadSingle(string path, bool abortOnError)
        {
            using (_logger?.BeginScope($"{new FileInfo(path).Name}"))
            {
                _logger?.LogInformation($"Loading plugin from {path}");

                PluginLoadDetails details = TryLoadSingleFile(path, abortOnError);

                return new PluginLoadResult(
                    path,
                    details.FileExists,
                    details.IsDll,
                    details.ContainsPlugins,
                    [details.Error],
                    details.LoadedAssembly
                );
            }
        }

        protected static string FormatErrorMessages(List<Exception> exceptions)
                            => string.Join('\n', exceptions.Select(ex => ex.Message));
        /// <summary>
        /// Handles errors based on the abortOnError flag.
        /// </summary>
        protected void HandleErrors(bool abortOnError, List<Exception> exceptions)
        {
            if (abortOnError)
            {
                _logger?.LogCritical("Aborting due to errors:");

                foreach (Exception ex in exceptions)
                {
                    _logger?.LogCritical(message: ex.Message);
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
        protected (Assembly? Assembly, bool ContainsPlugins) TryLoadAssembly(string path, List<Exception> exceptions)
        {
            Assembly? assembly = null;
            bool containsPlugins = false;

            try
            {
                assembly = Assembly.LoadFile(path);
                IEnumerable<Type> pluginTypes = assembly.GetTypes()
                    .Where(type => type.IsClass && typeof(IOrbitPlugin).IsAssignableFrom(type));

                containsPlugins = pluginTypes.Any();

                if (!containsPlugins)
                {
                    exceptions.Add(new ArgumentException($"File does not contain any plugins: {assembly.FullName}"));
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, $"Failed to load assembly from {path}");
                exceptions.Add(ex);
            }

            return (assembly, containsPlugins);
        }

        /// <summary>
        /// Attempts to load a plugin file and returns details about the operation.
        /// </summary>
        protected PluginLoadDetails TryLoadSingleFile(string path, bool abortOnError = false)
        {
            List<Exception> exceptions = new List<Exception>();
            var validator = new PluginFileValidator(path, _logger);

            // Validate the file path and type
            validator.Validate(exceptions);

            Assembly? loadedAssembly = null;
            bool containsPlugins = false;

            // Only attempt to load if basic validation passes
            if (validator.IsValid)
            {
                (loadedAssembly, containsPlugins) = TryLoadAssembly(path, exceptions);
            }

            // Handle errors if any occurred
            if (exceptions.Count != 0)
            {
                loadedAssembly = null;
                containsPlugins = false;
                HandleErrors(abortOnError, exceptions);
            }

            return new PluginLoadDetails(
                FileExists: validator.FileExists,
                IsDll: validator.IsDll,
                ContainsPlugins: containsPlugins,
                Error: FormatErrorMessages(exceptions),
                LoadedAssembly: loadedAssembly
            );
        }
    }
}