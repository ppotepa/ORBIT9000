namespace EngineTerminal.Proxies
{
    /// <summary>
    /// Record of a property change
    /// </summary>
    public class PropertyChangeRecord
    {
        public string PropertyPath { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}
