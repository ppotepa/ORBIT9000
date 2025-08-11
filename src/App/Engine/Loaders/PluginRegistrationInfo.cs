namespace ORBIT9000.Engine.Loaders
{
    public class PluginRegistrationInfo
    {
        public PluginRegistrationInfo(bool registered)
        {
            this.Registered = registered;
        }

        public bool AllowMultiple { get; internal set; } = false;
        public List<Task> Tasks { get; set; } = new List<Task>();
        public bool Registered { get; set; }
        public bool IsLoaded { get; internal set; }
    }
}