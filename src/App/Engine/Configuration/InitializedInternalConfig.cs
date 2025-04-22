using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Loaders.Plugin;
using ORBIT9000.Engine.Loaders.Plugin.Results;

namespace ORBIT9000.Engine.Configuration
{
    /// <summary>
    /// Represents the configuration for the Orbit Engine, including plugin information and default folder settings.
    /// </summary>
    public class InitializedInternalConfig
    {
        private readonly Raw.RawConfiguration _config;        
        private readonly ILogger<InitializedInternalConfig> _logger;
        private readonly IConfiguration configuration;
        private readonly IPluginLoader _loader; 

        public InitializedInternalConfig(ILogger<InitializedInternalConfig> logger, Raw.RawConfiguration config, IPluginLoader loader)
        {
            this._logger = logger;
            this._config = config;
            this._loader = loader;

            ArgumentNullException.ThrowIfNull(config);
            logger?.LogInformation("Creating OrbitEngineConfig from raw configuration.");

            try
            {
                DirectoryInfo defaultFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

                DefaultFolder = defaultFolder;
                var plugins = _loader.LoadPlugins(defaultFolder).Where(x => x.ContainsPlugins).ToArray();
                PluginInfo = plugins;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred while creating the OrbitEngineConfig from raw configuration.");
                throw;
            }
        }

        public DirectoryInfo DefaultFolder { get; set; }
        public AssemblyLoadResult[] PluginInfo { get; set; }
    }
}