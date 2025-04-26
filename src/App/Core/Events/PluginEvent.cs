namespace ORBIT9000.Core.Events
{
    public enum PluginEventType
    {
        Loaded,
        Unloaded,
        Failed,
        Activated,
        Deactivated
    }

    public class PluginEvent
    {
        public PluginEventType Type { get; set; }
        public Type PluginType { get; set; }
    }
}
