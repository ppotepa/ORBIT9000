using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.IO.Loaders.Plugin;

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

        private object PluginSource
        {
            get
            {
                if (_config.OrbitEngine.Plugins.ActivePlugins.Any()) return _config.OrbitEngine.Plugins.ActivePlugins;
                else return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            }
        }

        public InitializedInternalConfig(ILogger<InitializedInternalConfig> logger, Raw.RawConfiguration config, IPluginLoader loader)
        {
            this._logger = logger;
            this._config = config;
            this._loader = loader;

            ArgumentNullException.ThrowIfNull(config);
            logger?.LogInformation("Creating OrbitEngineConfig from raw configuration.");

            try
            {
                Plugins = _loader.LoadPlugins(PluginSource).Where(x => x.ContainsPlugins).ToArray();              
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred while creating the OrbitEngineConfig from raw configuration.");
                throw;
            }
        }

        public DirectoryInfo DefaultFolder { get; set; }
        public PluginInfo[] Plugins { get; set; }
    }
}