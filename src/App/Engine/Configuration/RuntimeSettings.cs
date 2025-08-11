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
<<<<<<< HEAD
<<<<<<< HEAD

        public RuntimeSettings() { }

        public RuntimeSettings(ILogger<RuntimeSettings> logger, RawEngineConfiguration config, IPluginLoader loader)
        {
            _config = config;
=======
        private readonly IPluginLoader _loader;        
=======
        private readonly IPluginLoader _loader;
>>>>>>> fd5a59f (Code Cleanup)
=======

        public RuntimeSettings() { }
>>>>>>> bfa6c2d (Try fix pipeline)

        public RuntimeSettings(ILogger<RuntimeSettings> logger, RawEngineConfiguration config, IPluginLoader loader)
        {
            this._config = config;
<<<<<<< HEAD
            this._loader = loader;
>>>>>>> 72c40c3 (Add Basic Event Handling for Settings)
=======
>>>>>>> bfa6c2d (Try fix pipeline)

            ArgumentNullException.ThrowIfNull(config);
            logger?.LogInformation("Creating OrbitEngineConfig from raw configuration.");

            try
            {
<<<<<<< HEAD
<<<<<<< HEAD
                PluginInfo[] loaded = [.. loader.LoadPlugins(PluginSource)];
                Plugins = [.. loaded];
=======
                var loaded = _loader.LoadPlugins(PluginSource).ToArray();
                Plugins = loaded.ToArray();
>>>>>>> 72c40c3 (Add Basic Event Handling for Settings)
                SharePluginScopes = config.SharePluginScopes;
                EnableTerminal = config.EnableTerminal;
=======
                PluginInfo[] loaded = [.. loader.LoadPlugins(this.PluginSource)];
                this.Plugins = [.. loaded];
                this.SharePluginScopes = config.SharePluginScopes;
                this.EnableTerminal = config.EnableTerminal;
>>>>>>> bfa6c2d (Try fix pipeline)
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
<<<<<<< HEAD
        public bool EnableTerminal { get; }
        public PluginInfo[] Plugins { get; set; }
        public bool SharePluginScopes { get; internal set; }
=======
        public PluginInfo[] Plugins { get; set; }
        public bool SharePluginScopes { get; internal set; }
        public bool EnableTerminal { get; }

>>>>>>> 72c40c3 (Add Basic Event Handling for Settings)
=======
        public bool EnableTerminal { get; }
        public PluginInfo[] Plugins { get; set; }
        public bool SharePluginScopes { get; internal set; }
>>>>>>> 147c461 (Refactor Program.CS)
        private object PluginSource
        {
            get
            {
                //TODO: Fix Active Plugins Source
<<<<<<< HEAD
<<<<<<< HEAD
                if (_config.Plugins.ActivePlugins.Length != 0) return _config.Plugins.ActivePlugins;
=======
                if (_config.Plugins.ActivePlugins.Any()) return _config.Plugins.ActivePlugins;
>>>>>>> 72c40c3 (Add Basic Event Handling for Settings)
=======
                if (this._config.Plugins.ActivePlugins.Length != 0) return this._config.Plugins.ActivePlugins;
>>>>>>> bfa6c2d (Try fix pipeline)
                else return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            }
        }
    }
}