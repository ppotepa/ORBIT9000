using EngineTerminal.EventArgs;
using EngineTerminal.Managers;
using Orbit9000.EngineTerminal.EventArgs;
using System.ComponentModel;

/// <summary>
/// This is an experimental terminal project for the Orbit9000 engine.
/// It is designed with minimal dependencies and libraries to focus on core functionality.
/// The primary focus is to create pipe communication and generic property change handling for better
/// display and monitoring.
/// </summary>
namespace Orbit9000.EngineTerminal
{
    public class ApplicationController
    {
        private readonly DataManager _dataManager;
        private readonly UIManager _uiManager;
        private readonly NamedPipeManager _pipeManager;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ApplicationController(
            DataManager dataManager,
            UIManager uiManager,
            NamedPipeManager pipeManager,
            CancellationTokenSource cancellationTokenSource)
        {
            _dataManager = dataManager;
            _uiManager = uiManager;
            _pipeManager = pipeManager;
            _cancellationTokenSource = cancellationTokenSource;

            _dataManager.Initialize();
            _uiManager.Initialize(_dataManager.ExampleData);
            _uiManager.PropertyChanged += OnPropertyChangedHandler;

            _pipeManager.DataReceived += OnDataReceived;
            _pipeManager.StatusChanged += OnPipeStatusChanged;
        }

        public void Run()
        {
            _pipeManager.StartProcessing(_cancellationTokenSource.Token);

            _uiManager.Run();
        }

        public void Shutdown()
        {
            _cancellationTokenSource.Cancel();
            _pipeManager.Dispose();
        }

        private void OnPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            _uiManager.UpdateBindingFromPropertyChange(sender, e);
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            _dataManager.UpdateData(e.ReceivedData, _uiManager.Bindings);
            _uiManager.UpdateUIFromData(_dataManager.ExampleData);
        }

        private void OnPipeStatusChanged(object sender, StatusChangedEventArgs e)
        {
            _uiManager.UpdateStatusMessage(e.StatusMessage);
        }
    }
}