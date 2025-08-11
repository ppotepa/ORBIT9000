namespace ORBIT9000.Core.Events;
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
    public required Type PluginType { get; set; }
    public required PluginEventType Type { get; set; }
}
