using EngineTerminal.Contracts;
using ORBIT9000.Core.Environment;
using ORBIT9000.Core.Models.Pipe.ORBIT9000.Core.Models.Pipe;
using System.Diagnostics;
using System.Threading.Channels;
using static EngineTerminal.Managers.UIManager;

namespace Orbit9000.EngineTerminal
{
    public class ApplicationController : Disposable
    {
        #region Fields

        private readonly IDataManager _dataManager;
        private readonly ChannelReader<ExampleData> _dataReader;
        private readonly IPipeManager _pipeManager;
        private readonly ChannelReader<string> _statusReader;
        private readonly CancellationTokenSource _tokenSource = new();
        private readonly IUIManager _uiManager;

        public ApplicationController(
            IDataManager dataManager,
            IUIManager uiManager,
            IPipeManager pipeManager,
            Channel<ExampleData> dataChannel,
            Channel<string> statusChannel)
        {
            if (dataManager == null) throw new ArgumentNullException(nameof(dataManager));
            if (uiManager == null) throw new ArgumentNullException(nameof(uiManager));
            if (pipeManager == null) throw new ArgumentNullException(nameof(pipeManager));
            if (dataChannel == null) throw new ArgumentNullException(nameof(dataChannel));
            if (statusChannel == null) throw new ArgumentNullException(nameof(statusChannel));

            _dataManager = dataManager;
            _uiManager = uiManager;
            _pipeManager = pipeManager;

            _dataReader = dataChannel.Reader;
            _statusReader = statusChannel.Reader;

            this.PipeDataReceived += _uiManager.UpdateUIFromData;
        }

        #endregion Constructors

        #region Events

        public event EventHandler<IReadOnlyList<BindingAction>> PipeDataReceived;

        #endregion Events

        #region Methods

        public async Task RunAsync()
        {
            _dataManager.Initialize();
            _uiManager.Initialize(_dataManager.Data);

            var pipeTask = _pipeManager.StartProcessingAsync(_tokenSource.Token);

            var dataTask = Task.Run(GetData, _tokenSource.Token);
            var statusTask = Task.Run(GetStatus, _tokenSource.Token);

            _uiManager.Run();

            _uiManager.UpdateStatusMessage("Initializing UI");
            await Task.WhenAll(pipeTask, dataTask, statusTask);
            await _tokenSource.CancelAsync();
        }

        protected virtual void OnDataReceived(IReadOnlyList<BindingAction> actions)
        {
            _uiManager.UpdateCurrentMethod($"Data Received. {actions.Count} update actions.");

            PipeDataReceived?.Invoke(this, actions);
        }

        private async Task GetData()
        {
            await foreach (var newData in _dataReader.ReadAllAsync(_tokenSource.Token))
            {
                _uiManager.UpdateCurrentMethod("Obtaining data");

                var stopwatch = new Stopwatch();
                {
                    stopwatch.Start();

                    var updates = _dataManager.GetUpdates(newData, _uiManager.GridBindings ?? new Dictionary<string, Terminal.Gui.CustomViews.Misc.ValueBinding>());

                    if (updates.Any())
                    {
                        _uiManager.UpdateCurrentMethod($"Processing updates {updates.Count}.");
                        _uiManager.UpdateUIFromData(this, updates);
                        OnDataReceived(updates);
                    }

                    stopwatch.Stop();

                    _uiManager.UpdateStatusMessage(null, $"Last Update took : {stopwatch.ElapsedMilliseconds}ms");
                    await Task.Delay(100);
                }
            }
        }
        private async Task GetStatus()
        {
            await foreach (var status in _statusReader.ReadAllAsync(_tokenSource.Token))
            {
                _uiManager.UpdateStatusMessage(status);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion Methods
    }
}