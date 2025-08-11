<<<<<<< HEAD
<<<<<<< HEAD
﻿using EngineTerminal.Contracts;
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

            _dataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
            _uiManager = uiManager ?? throw new ArgumentNullException(nameof(uiManager));
            _pipeManager = pipeManager ?? throw new ArgumentNullException(nameof(pipeManager));

            _dataReader = dataChannel.Reader;
            _statusReader = statusChannel.Reader;

            PipeDataReceived += _uiManager.UpdateUIFromData;
        }

        #endregion Fields

        #region Events

        public event EventHandler<IReadOnlyList<BindingAction>> PipeDataReceived;

        #endregion Events

        #region Methods

        public async Task RunAsync()
        {
            _dataManager.Initialize();
            _uiManager.Initialize(_dataManager.Data!);

            Task pipeTask = _pipeManager.StartProcessingAsync(_tokenSource.Token);

            Task dataTask = Task.Run(GetData, _tokenSource.Token);
            Task statusTask = Task.Run(GetStatus, _tokenSource.Token);

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
            await foreach (ExampleData newData in _dataReader.ReadAllAsync(_tokenSource.Token))
            {
                _uiManager.UpdateCurrentMethod("Obtaining data");
                await Process(newData);
            }
        }

        private async Task Process(ExampleData newData)
        {
            Stopwatch stopwatch = new();

            stopwatch.Start();

            IReadOnlyList<BindingAction> updates = _dataManager.GetUpdates(newData, _uiManager.GridBindings ?? []);

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

        private async Task GetStatus()
        {
            await foreach (string status in _statusReader.ReadAllAsync(_tokenSource.Token))
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
=======
﻿using EngineTerminal.EventArgs;
using EngineTerminal.Managers;
using Orbit9000.EngineTerminal.EventArgs;
using System.ComponentModel;
=======
﻿using EngineTerminal.Contracts;
using EngineTerminal.Managers;
using ORBIT9000.Core.Models.Pipe;
using System.Diagnostics;
using System.Threading.Channels;
<<<<<<< HEAD
>>>>>>> 5ae5b98 (Add Inversion of Control)
=======
using static EngineTerminal.Managers.UIManager;
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)

namespace Orbit9000.EngineTerminal
{
    public class ApplicationController
    {
        #region Fields

        private readonly IDataManager _dataManager;
        private readonly ChannelReader<ExampleData> _dataReader;
        private readonly IPipeManager _pipeManager;
        private readonly ChannelReader<string> _statusReader;
        private readonly CancellationTokenSource _tokenSource = new();
        private readonly IUIManager _uiManager;

        #endregion Fields

        #region Constructors

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
                    
                    var updates = _dataManager.GetUpdates(newData, _uiManager.Bindings);

                    if (updates.Any())
                    {
                        _uiManager.UpdateCurrentMethod($"Processing updates {updates.Count}.");
                        _uiManager.UpdateUIFromData(this, updates);
                        OnDataReceived(updates);
                    }

                    stopwatch.Stop();

                    _uiManager.UpdateStatusMessage(null, $"Last Update took : { stopwatch.ElapsedMilliseconds}ms");
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
<<<<<<< HEAD
>>>>>>> 80f2a0e (Split Responsibilities To Managers)
=======

        #endregion Methods
>>>>>>> dceb24b (Rework Translator into Property Grid View with event handling)
    }
}