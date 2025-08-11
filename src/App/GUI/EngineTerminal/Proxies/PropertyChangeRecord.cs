namespace EngineTerminal.Proxies
{
    /// <summary>
    /// Record of a property change
    /// </summary>
    public class PropertyChangeRecord
    {
<<<<<<< HEAD
        public required string PropertyPath { get; set; }
        public required object? OldValue { get; set; }
        public required object? NewValue { get; set; }
=======
        public string PropertyPath { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
    }
}
