namespace ORBIT9000.Engine.Configuration.Raw
{
    internal class PluginEngineConfig
    {
        public required string[] ActivePlugins { get; set; } = Array.Empty<string>();
        public required bool AbortOnError { get; set; }
        public required bool LoadAsBinary { get; set; }

    }
}
