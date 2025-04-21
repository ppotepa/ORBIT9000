
namespace ORBIT9000.Engine
{
    public class PluginActivationInfo
    {
        public PluginActivationInfo(bool registered)
        {
            this.Registered = registered;
        }

        public bool AllowMultiple { get; internal set; } = false;
        public List<Task> Instances { get; set; } = new List<Task>();
        public bool Registered { get; set; }
    }
}