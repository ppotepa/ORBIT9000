using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Factories;
using ORBIT9000.Engine.Loaders.Plugin.Results;
using ORBIT9000.Engine.Loaders.Plugin.Implementations;

namespace ORBIT9000.Engine.Configuration
{
    /// <summary>
    /// Represents the configuration for the Orbit Engine, including plugin information and default folder settings.
    /// </summary>
    public class OrbitEngineConfig
    {
        public required PluginLoadResult[] Plugins { get; set; }
        public required DirectoryInfo DefaultFolder { get; set; }

        internal static OrbitEngineConfig? FromRaw(RawOrbitEngineConfig rawConfig, ILogger? logger = default)
        {
            logger?.LogInformation("Creating OrbitEngineConfig from raw configuration.");

            try
            {
                string[] plugins = rawConfig.OrbitEngine.Plugins.ActivePlugins;
                DirectoryInfo defaultFolder = new DirectoryInfo("./plugins");

                return new OrbitEngineConfig
                {
                    DefaultFolder = defaultFolder,
                    Plugins = PluginLoaderFActory.Load(rawConfig.OrbitEngine.Plugins, logger).ToArray()                        
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