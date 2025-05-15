namespace ORBIT9000.Engine.Configuration.Raw
{
    public class PluginsConfiguration
    {
        public required bool AbortOnError { get; set; }
        public required string[] ActivePlugins { get; set; } = [];
        public required bool LoadAsBinary { get; set; }
    }

    public class RawEngineConfiguration
    {
        public bool EnableTerminal { get; set; } = false;
        public required PluginsConfiguration Plugins { get; set; }
        public bool SharePluginScopes { get; set; }
        public bool UseDefaultFolder => Plugins.ActivePlugins.Length == 0;
        public required object Database { get; set; }
    }

    public class Database
    {
        public ConnectionStringInfo? Debug { get; set; }
        public ConnectionStringInfo? Release { get; set; }
    }

    public class ConnectionStringInfo
    {
        public string? ConnectionString { get; set; }
        public string? Provider { get; set; }
    }
}