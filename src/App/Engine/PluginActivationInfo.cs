
namespace ORBIT9000.Engine
{
    internal class PluginActivationInfo
    {
        public PluginActivationInfo(bool registered)
        {
            this.Registered = registered;
        }

        public bool Registered { get; set; }
        public List<Task> Instances { get; set; } = new List<Task>();
        public bool AllowMultiple { get; internal set; } = true;
    }
}