namespace ORBIT9000.Engine.Configuration.Raw
{
    internal class RawOrbitEngine
    {
        public required RawPlugins Plugins { get; set; }
        public bool UseDefaultFolder => this.Plugins.ActivePlugins.Length == 0;
    }
}