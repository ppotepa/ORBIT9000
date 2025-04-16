namespace ORBIT9000.Engine.Configuration.Raw
{
    internal class RawPlugins
    {
        public required string[] ActivePlugins { get; set; } = Array.Empty<string>();        
        public required bool AbortOnError { get; set; }

    }
}
