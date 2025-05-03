using EngineTerminal.Managers;

/// <summary>
/// This is an experimental terminal project for the Orbit9000 engine.
/// It is designed with minimal dependencies and libraries to focus on core functionality.
/// The primary focus is to create pipe communication and generic property change handling for better
/// display and monitoring.
/// </summary>
namespace Orbit9000.EngineTerminal
{
    public class Program
    {
        private static void Main(string[] args)
        {
            using var cancellationTokenSource = new CancellationTokenSource();

            var dataManager = new DataManager();
            var uiManager = new UIManager();
            var pipeManager = new NamedPipeManager(".", "OrbitEngine");

            var applicationController = new ApplicationController(
                dataManager,
                uiManager,
                pipeManager,
                cancellationTokenSource
            );

            try
            {
                applicationController.Run();
            }
            finally
            {
                applicationController.Shutdown();
            }
        }
    }
}