namespace ORBIT9000.Engine.Configuration.Raw
{
    public class RawConfiguration
    {
        public required EngineConfiguration OrbitEngine { get; set; }
    }

    public class EngineConfiguration
    {
        public required PluginsConfiguration Plugins { get; set; }
        public bool UseDefaultFolder => this.Plugins.ActivePlugins.Length == 0;
    }

    public class PluginsConfiguration
    {
        public required bool AbortOnError { get; set; }
        public required string[] ActivePlugins { get; set; } = Array.Empty<string>();
        public required bool LoadAsBinary { get; set; }
    }
}