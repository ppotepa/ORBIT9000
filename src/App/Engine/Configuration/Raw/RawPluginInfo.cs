namespace ORBIT9000.Engine.Configuration.Raw
{
    internal class RawPluginInfo
    {
        public required string[] ActivePlugins { get; set; } = Array.Empty<string>();        
        public required bool AbortOnError { get; set; }

    }
}
