using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Loaders.Plugin.Results;
using ORBIT9000.Engine.Loaders.Plugin.Strategies;
using ORBIT9000.Engine.Loaders.Plugin.Strategies.ORBIT9000.Engine.Loaders.Plugin.Strategies;

namespace ORBIT9000.Engine.Loaders.Plugin
{
    /// <summary>
    /// Factory class for plugin loading strategies.
    /// </summary>
    internal static class PluginLoader
    {
        /// <summary>
        /// Resolves the appropriate plugin loading strategy based on configuration.
        /// </summary>
        /// <remarks>
        /// If plugins with explicitly defined paths are provided in <see cref="RawOrbitEngineConfig"/>,
        /// only those specified plugins will be loaded using <see cref="LoadFromStringArrayStrategy"/>.
        /// Otherwise, plugins are loaded from the default directory using <see cref="LoadFromDirectoryStrategy"/>.
        /// <para>
        /// <b> Active Plugins are Recommended for DEBUG and TESTING </b>
        /// </para>
        /// </remarks>
        /// <param name="rawConfig">The raw configuration containing plugin settings.</param>
        /// <param name="logger">Optional logger for recording plugin loading operations.</param>
        /// <returns>An instance of <see cref="PluginLoadingStrategy"/> appropriate for the configuration.</returns>
        public static IEnumerable<PluginLoadResult> Load(Configuration.Raw.RawOrbitEngineConfig rawConfig, ILogger? logger = default)
        {
            return rawConfig.OrbitEngine.Plugins.ActivePlugins.Length != 0 ?
                new LoadFromStringArrayStrategy(rawConfig, logger).LoadPlugins(rawConfig.OrbitEngine.Plugins.ActivePlugins) :
                new LoadFromDirectoryStrategy(rawConfig, logger).LoadPlugins(new DirectoryInfo("./Plugins"));
        }
    }
}
