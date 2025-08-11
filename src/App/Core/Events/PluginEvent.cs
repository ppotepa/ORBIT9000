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
        public Type PluginType { get; set; }
        public PluginEventType Type { get; set; }
    }
}
