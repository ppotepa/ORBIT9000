namespace ORBIT9000.Engine.Configuration.Raw
{
    public class RawPlugins
    {
        public required string[] ActivePlugins { get; set; }
        public required string DefaultFolder { get; set; }
        public required bool SkipMissing { get; set; }
    }
}