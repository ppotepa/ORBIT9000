using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Loaders.Plugin;
using ORBIT9000.Engine.Loaders.Plugin.Results;

namespace ORBIT9000.Engine.Configuration
{
    /// <summary>
    /// Represents the configuration for the Orbit Engine, including plugin information and default folder settings.
    /// </summary>
    public class OrbitEngineConfig
    {
        public required DirectoryInfo DefaultFolder { get; set; }
        public required PluginLoadResult[] PluginInfo { get; set; }
        internal static OrbitEngineConfig? FromRaw(RawOrbitEngineConfig rawConfig, ILogger? logger = default, IServiceCollection? services = null)
        {

            logger?.LogInformation("Creating OrbitEngineConfig from raw configuration.");

            try
            {
                string[] plugins = rawConfig.OrbitEngine.Plugins.ActivePlugins;
                DirectoryInfo defaultFolder = new DirectoryInfo("./plugins");

                return new OrbitEngineConfig
                {
                    DefaultFolder = defaultFolder,
                    PluginInfo = PluginLoaderFactory.Load(rawConfig.OrbitEngine.Plugins, logger)
                                .Where(result => result.ContainsPlugins)
                                .ToArray()
                };
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred while creating the OrbitEngineConfig from raw configuration.");
                throw;
            }
        }
    }
}