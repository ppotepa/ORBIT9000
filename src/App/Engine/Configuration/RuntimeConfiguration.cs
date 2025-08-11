#nullable disable
using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.IO.Loaders.Plugin;

namespace ORBIT9000.Engine.Configuration
{
    /// <summary>
    /// Represents the configuration for the Orbit Engine, including plugin information and default folder settings.
    /// </summary>
    public class RuntimeConfiguration
    {
        private readonly RawEngineConfiguration _config;
        private readonly IPluginLoader _loader;        

        public RuntimeConfiguration(ILogger<RuntimeConfiguration> logger, RawEngineConfiguration config, IPluginLoader loader)
        {
            this._config = config;
            this._loader = loader;

            ArgumentNullException.ThrowIfNull(config);
            logger?.LogInformation("Creating OrbitEngineConfig from raw configuration.");

            try
            {
                var loaded = _loader.LoadPlugins(PluginSource).ToArray();
                Plugins = loaded.ToArray();
                SharePluginScopes = config.SharePluginScopes;
                EnableTerminal = config.EnableTerminal;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "An error occurred while Loading Plugins.");

                if (config.Plugins.AbortOnError)
                {
                    throw;
                }
            }
        }

        public DirectoryInfo DefaultFolder { get; set; }
        public PluginInfo[] Plugins { get; set; }
        public bool SharePluginScopes { get; internal set; }
        public bool EnableTerminal { get; }

        private object PluginSource
        {
            get
            {
                if (_config.Plugins.ActivePlugins.Any()) return _config.Plugins.ActivePlugins;
                else return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            }
        }
    }
}