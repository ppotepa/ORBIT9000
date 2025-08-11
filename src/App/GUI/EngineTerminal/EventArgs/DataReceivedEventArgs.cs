using ORBIT9000.Core.Models.Pipe;

/// <summary>
/// This is an experimental terminal project for the Orbit9000 engine.  
/// It is designed with minimal dependencies and libraries to focus on core functionality.  
/// The primary focus is to create pipe communication and generic property change handling for better
/// display and monitoring.
/// </summary>
namespace Orbit9000.EngineTerminal.EventArgs
{
    public class DataReceivedEventArgs : System.EventArgs
    {
        public ExampleData ReceivedData { get; }

        public DataReceivedEventArgs(ExampleData receivedData)
        {
            ReceivedData = receivedData;
        }
    }
}
