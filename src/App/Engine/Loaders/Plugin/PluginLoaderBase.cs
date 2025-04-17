using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Engine.Loaders.Plugin.Details;
using ORBIT9000.Engine.Loaders.Plugin.Results;
using ORBIT9000.Engine.Loaders.Plugin.Validation;
using System.Reflection;

namespace ORBIT9000.Engine.Loaders.Plugin
{
    /// <summary>
    /// Base abstract class for plugin loading strategies.
    /// </summary>
    /// <typeparam name="TSource">The source type from which plugins are loaded.</typeparam>
    internal abstract class PluginLoaderBase<TSource>
    {
        /// <summary>
        /// Logger instance for recording plugin loading operations.
        /// </summary>
        protected readonly ILogger? _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginLoaderBase{TSource}"/> class.
        /// </summary>
        /// <param name="logger">Optional logger for recording plugin loading operations.</param>
        protected PluginLoaderBase(ILogger? logger = default)
        {
            _logger = logger;
        }

        /// <summary>
        /// Loads plugins from the specified source.
        /// </summary>
        /// <param name="source">The source from which to load plugins.</param>
        /// <returns>A collection of plugin load results.</returns>
        public abstract IEnumerable<PluginLoadResult> LoadPlugins(TSource source);

        /// <summary>
        /// Loads a single plugin from the specified path.
        /// </summary>
        /// <param name="path">Path to the plugin file.</param>
        /// <param name="abortOnError">Whether to throw exceptions on error.</param>
        /// <returns>A result containing information about the loaded plugin.</returns>
        protected PluginLoadResult LoadSingle(string path, bool abortOnError)
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

        /// <summary>
        /// Formats a list of exceptions into a single error message.
        /// </summary>
        private string FormatErrorMessages(List<Exception> exceptions)
            => string.Join('\n', exceptions.Select(ex => ex.Message));

        /// <summary>
        /// Handles errors based on the abortOnError flag.
        /// </summary>
        private void HandleErrors(bool abortOnError, List<Exception> exceptions)
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

        /// <summary>
        /// Attempts to load an assembly and check if it contains plugins.
        /// </summary>
        private (Assembly? Assembly, bool ContainsPlugins) TryLoadAssembly(string path, List<Exception> exceptions)
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
                    _logger?.LogWarning("File does not contain any plugins.");
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
        private PluginLoadDetails TryLoadSingleFile(string path, bool abortOnError = false)
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
