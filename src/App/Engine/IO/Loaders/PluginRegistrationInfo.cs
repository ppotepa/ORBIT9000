#nullable disable
namespace ORBIT9000.Engine.IO.Loaders
{
    public class PluginRegistrationInfo
    {
        public PluginRegistrationInfo(bool registered)
        {
            this.Registered = registered;
        }

        public bool AllowMultiple { get; internal set; } = false;
        public bool IsLoaded { get; internal set; }
        public bool Registered { get; set; }
        public List<Task> Tasks { get; set; } = new List<Task>();
        public Type Type { get; internal set; }
    }
}