/// <summary>
/// This is an experimental terminal project for the Orbit9000 engine.  
/// It is designed with minimal dependencies and libraries to focus on core functionality.  
/// The primary focus is to create pipe communication and generic property change handling for better
/// display and monitoring.
/// </summary>
namespace EngineTerminal.EventArgs
{
    public class StatusChangedEventArgs : System.EventArgs
    {
        public string StatusMessage { get; }

        public StatusChangedEventArgs(string statusMessage)
        {
            StatusMessage = statusMessage;
        }
    }
}
