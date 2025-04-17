using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Loaders.Plugin.Results;
using ORBIT9000.Engine.Loaders.Plugin.Implementations;

namespace ORBIT9000.Engine.Factories
{
    /// <summary>
    /// Factory class for plugin loading strategies.
    /// </summary>
    internal static class PluginLoaderFActory
    {
        /// <summary>
        /// Resolves the appropriate plugin loading strategy based on configuration.
        /// </summary>
        /// <remarks>
        /// If plugins with explicitly defined paths are provided in <see cref="RawOrbitEngineConfig"/>,
        /// only those specified plugins will be loaded using <see cref="StringArrayPluginLoader"/>.
        /// Otherwise, plugins are loaded from the default directory using <see cref="DirectoryPluginLoader"/>.
        /// <para>
        /// <b> Active Plugins are Recommended for DEBUG and TESTING </b>
        /// </para>
        /// </remarks>
        /// <param name="rawConfig">The raw configuration containing plugin settings.</param>
        /// <param name="logger">Optional logger for recording plugin loading operations.</param>
        /// <returns>An instance of <see cref="PluginLoadingStrategy"/> appropriate for the configuration.</returns>
        public static IEnumerable<PluginLoadResult> Load(RawPlugins rawPlugins, ILogger? logger = default)
        {
            return rawPlugins.ActivePlugins.Length != 0 ?
                new StringArrayPluginLoader(logger).LoadPlugins(rawPlugins.ActivePlugins) :
                new DirectoryPluginLoader(logger).LoadPlugins(new DirectoryInfo("./Plugins"));
        }
    }
}
