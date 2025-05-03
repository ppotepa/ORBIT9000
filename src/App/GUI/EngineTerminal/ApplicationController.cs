using EngineTerminal.Contracts;
using ORBIT9000.Core.Models.Pipe;
using System.Threading.Channels;

namespace Orbit9000.EngineTerminal
{
    public class ApplicationController
    {
        private readonly CancellationTokenSource _cts = new();

        private readonly IDataManager _dataManager;
        private readonly ChannelReader<ExampleData> _dataReader;
        private readonly IPipeManager _pipeManager;
        private readonly ChannelReader<string> _statusReader;

        private readonly IUIManager _uiManager;


        public ApplicationController(
            IDataManager dataManager,
            IUIManager uiManager,
            IPipeManager pipeManager,
            Channel<ExampleData> dataChannel,
            Channel<string> statusChannel)
        {
            _dataManager = dataManager;
            _uiManager = uiManager;
            _pipeManager = pipeManager;
            _dataReader = dataChannel.Reader;
            _statusReader = statusChannel.Reader;
        }

        public async Task RunAsync()
        {
            _dataManager.Initialize();
            _uiManager.Initialize(_dataManager.ExampleData);

            var pipeTask = _pipeManager.StartProcessingAsync(_cts.Token);

            var dataTask = Task.Run(async () =>
            {
                await foreach (var newData in _dataReader.ReadAllAsync(_cts.Token))
                {
                    var updates = _dataManager.GetUpdates(newData, _uiManager.Bindings);
                    _uiManager.UpdateUIFromData(updates);
                }
            }, _cts.Token);

            var statusTask = Task.Run(async () =>
            {
                await foreach (var status in _statusReader.ReadAllAsync(_cts.Token))
                    _uiManager.UpdateStatusMessage(status);
            }, _cts.Token);

            _uiManager.Run();

            _cts.Cancel();
            await Task.WhenAll(pipeTask, dataTask, statusTask);
        }
    }
}