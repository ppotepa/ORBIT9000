#nullable disable
namespace ORBIT9000.Engine.IO.Loaders
{
    public class PluginRegistrationInfo(bool registered)
    {
        public bool AllowMultiple { get; internal set; } = false;
        public bool IsLoaded { get; internal set; }
        public bool Registered { get; set; } = registered;
        public List<Task> Tasks { get; set; } = [];
        public Type Type { get; internal set; }
    }
}