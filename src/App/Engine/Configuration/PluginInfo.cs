using System.Reflection;

namespace ORBIT9000.Engine.Configuration
{
    public class PluginInfo
    {
        public Assembly Assembly { get; internal set; }
        public Type PluginType { get; internal set; }
        public FileInfo FileInfo { get; internal set; }
        public bool ContainsPlugins => PluginType is not null;

        public bool Activated { get; internal set; }
        public bool IsSingleton { get; internal set; }
    }
}