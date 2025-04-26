using Microsoft.Extensions.Options;

namespace ORBIT9000.Engine.Configuration.Raw
{
    public class PluginsConfiguration
    {
        public required bool AbortOnError { get; set; }
        public required string[] ActivePlugins { get; set; } = Array.Empty<string>();
        public required bool LoadAsBinary { get; set; }
    }

    public class RawEngineConfiguration
    {
        public required PluginsConfiguration Plugins { get; set; }
        public bool SharePluginScopes { get; set; }
        public bool UseDefaultFolder => this.Plugins.ActivePlugins.Length == 0;
        public bool EnableTerminal { get; set; } = false;   
    }
}