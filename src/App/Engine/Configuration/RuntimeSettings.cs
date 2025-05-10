#nullable disable
using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.IO.Loaders.Plugin;

namespace ORBIT9000.Engine.Configuration
{
    /// <summary>
    /// Represents the configuration for the Orbit Engine, including plugin information and default folder settings.
    /// </summary>
    public class RuntimeSettings
    {
        private readonly RawEngineConfiguration _config;

        public RuntimeSettings() { }

        public RuntimeSettings(ILogger<RuntimeSettings> logger, RawEngineConfiguration config, IPluginLoader loader)
        {
            this._config = config;

            ArgumentNullException.ThrowIfNull(config);
            logger?.LogInformation("Creating OrbitEngineConfig from raw configuration.");

            try
            {
                PluginInfo[] loaded = [.. loader.LoadPlugins(this.PluginSource)];
                this.Plugins = [.. loaded];
                this.SharePluginScopes = config.SharePluginScopes;
                this.EnableTerminal = config.EnableTerminal;
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
        public bool EnableTerminal { get; }
        public PluginInfo[] Plugins { get; set; }
        public bool SharePluginScopes { get; internal set; }
        private object PluginSource
        {
            get
            {
                //TODO: Fix Active Plugins Source
                if (this._config.Plugins.ActivePlugins.Length != 0) return this._config.Plugins.ActivePlugins;
                else return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            }
        }
    }
}