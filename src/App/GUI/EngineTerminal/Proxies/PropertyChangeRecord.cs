namespace EngineTerminal.Proxies
{
    /// <summary>
    /// Record of a property change
    /// </summary>
    public class PropertyChangeRecord
    {
        public required string PropertyPath { get; set; }
        public required object? OldValue { get; set; }
        public required object? NewValue { get; set; }
    }
}
