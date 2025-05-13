using EngineTerminal.Contracts;
using ORBIT9000.Core.Environment;
using ORBIT9000.Core.Models.Pipe;
using System.Diagnostics;
using System.Threading.Channels;
using static EngineTerminal.Managers.UIManager;

namespace EngineTerminal
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
            ArgumentNullException.ThrowIfNull(dataChannel);
            ArgumentNullException.ThrowIfNull(statusChannel);

            this._dataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
            this._uiManager = uiManager ?? throw new ArgumentNullException(nameof(uiManager));
            this._pipeManager = pipeManager ?? throw new ArgumentNullException(nameof(pipeManager));

            this._dataReader = dataChannel.Reader;
            this._statusReader = statusChannel.Reader;

            PipeDataReceived += this._uiManager.UpdateUIFromData;
        }

        #endregion Fields

        #region Events

        public event EventHandler<IReadOnlyList<BindingAction>> PipeDataReceived;

        #endregion Events

        #region Methods

        public async Task RunAsync()
        {
            this._dataManager.Initialize();
            this._uiManager.Initialize(this._dataManager.Data!);

            Task pipeTask = this._pipeManager.StartProcessingAsync(this._tokenSource.Token);

            Task dataTask = Task.Run(this.GetData, this._tokenSource.Token);
            Task statusTask = Task.Run(this.GetStatus, this._tokenSource.Token);

            this._uiManager.Run();

            this._uiManager.UpdateStatusMessage("Initializing UI");
            await Task.WhenAll(pipeTask, dataTask, statusTask);
            await this._tokenSource.CancelAsync();
        }

        protected virtual void OnDataReceived(IReadOnlyList<BindingAction> actions)
        {
            this._uiManager.UpdateCurrentMethod($"Data Received. {actions.Count} update actions.");

            PipeDataReceived?.Invoke(this, actions);
        }

        private async Task GetData()
        {
            await foreach (ExampleData newData in this._dataReader.ReadAllAsync(this._tokenSource.Token))
            {
                this._uiManager.UpdateCurrentMethod("Obtaining data");
                await this.Process(newData);
            }
        }

        private async Task Process(ExampleData newData)
        {
            Stopwatch stopwatch = new();

            stopwatch.Start();

            IReadOnlyList<BindingAction> updates = this._dataManager.GetUpdates(newData, this._uiManager.GridBindings ?? []);

            if (updates.Any())
            {
                this._uiManager.UpdateCurrentMethod($"Processing updates {updates.Count}.");
                this._uiManager.UpdateUIFromData(this, updates);
                this.OnDataReceived(updates);
            }

            stopwatch.Stop();

            this._uiManager.UpdateStatusMessage(null, $"Last Update took : {stopwatch.ElapsedMilliseconds}ms");
            await Task.Delay(100);
        }

        private async Task GetStatus()
        {
            await foreach (string status in this._statusReader.ReadAllAsync(this._tokenSource.Token))
            {
                this._uiManager.UpdateStatusMessage(status);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._tokenSource.Cancel();
                this._tokenSource.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion Methods
    }
}