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
<<<<<<< HEAD

        public RuntimeSettings() { }

        public RuntimeSettings(ILogger<RuntimeSettings> logger, RawEngineConfiguration config, IPluginLoader loader)
        {
            _config = config;
=======
        private readonly IPluginLoader _loader;        

        public RuntimeSettings(ILogger<RuntimeSettings> logger, RawEngineConfiguration config, IPluginLoader loader)
        {
            this._config = config;
            this._loader = loader;
>>>>>>> 72c40c3 (Add Basic Event Handling for Settings)

            ArgumentNullException.ThrowIfNull(config);
            logger?.LogInformation("Creating OrbitEngineConfig from raw configuration.");

            try
            {
<<<<<<< HEAD
                PluginInfo[] loaded = [.. loader.LoadPlugins(PluginSource)];
                Plugins = [.. loaded];
=======
                var loaded = _loader.LoadPlugins(PluginSource).ToArray();
                Plugins = loaded.ToArray();
>>>>>>> 72c40c3 (Add Basic Event Handling for Settings)
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
<<<<<<< HEAD
        public bool EnableTerminal { get; }
        public PluginInfo[] Plugins { get; set; }
        public bool SharePluginScopes { get; internal set; }
=======
        public PluginInfo[] Plugins { get; set; }
        public bool SharePluginScopes { get; internal set; }
        public bool EnableTerminal { get; }

>>>>>>> 72c40c3 (Add Basic Event Handling for Settings)
        private object PluginSource
        {
            get
            {
                //TODO: Fix Active Plugins Source
<<<<<<< HEAD
                if (_config.Plugins.ActivePlugins.Length != 0) return _config.Plugins.ActivePlugins;
=======
                if (_config.Plugins.ActivePlugins.Any()) return _config.Plugins.ActivePlugins;
>>>>>>> 72c40c3 (Add Basic Event Handling for Settings)
                else return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            }
        }
    }
}