namespace ORBIT9000.Engine.Configuration.Raw
{
    internal class OrbitEngineConfig
    {
        public required PluginEngineConfig Plugins { get; set; }
        public bool UseDefaultFolder => this.Plugins.ActivePlugins.Length == 0;
    }
}