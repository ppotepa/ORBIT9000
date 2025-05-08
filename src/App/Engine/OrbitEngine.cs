using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Abstractions.Scheduling;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Runtime.State;

namespace ORBIT9000.Engine
{
    public partial class OrbitEngine
    {
        #region Fields

        private readonly ILogger<OrbitEngine> _logger;
        private readonly Thread _mainThread;
        private readonly IPluginProvider _pluginProvider;
        private readonly IServiceProvider _serviceProvider;
        private IScheduler _scheduler;

        public IScheduler Scheduler
        {
            get => _scheduler;
            private set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Scheduler instance cannot be null.");
                _scheduler = value;
            }
        }

        private RuntimeSettings _configuration;

        #endregion Fields

        #region Constructors

        public OrbitEngine(
                    ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider,
            RuntimeSettings configuration,
            IPluginProvider pluginProvider,
            IScheduler scheduler
            )
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(loggerFactory);
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(pluginProvider);
            ArgumentNullException.ThrowIfNull(scheduler);

            _logger = loggerFactory.CreateLogger<OrbitEngine>()
                ?? throw new InvalidOperationException("Logger could not be created.");

            _mainThread = new Thread(Strategies.Running.Default.EngineStartupStrategy);
            _mainThread.IsBackground = true;

            _mainThread.Name = "MainEngineThread";

            _pluginProvider = pluginProvider;
            _serviceProvider = serviceProvider;

            Scheduler = scheduler;

            IsInitialized = true;
            IsRunning = true;

            _configuration = configuration;
            _logger.LogInformation("Engine initialized with configuration: {Configuration}", configuration);
        }

        #endregion Constructors

        #region Properties

        public bool IsInitialized { get; }
        public bool IsRunning { get; private set; }
        public IPluginProvider PluginProvider { get => _pluginProvider; }
        public IServiceProvider ServiceProvider { get => _serviceProvider; }
        internal RuntimeSettings Configuration { get => _configuration; set => _configuration = value; }

        #endregion Properties

        #region Methods

        public void Start()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Engine has not been initialized.");

            _logger.LogInformation("Starting engine thread...");
            _mainThread.Start(_serviceProvider.GetAutofacRoot().Resolve<EngineState>());

            while (IsRunning)
            {
                Thread.Sleep(100);
            }
        }

        #endregion Methods
    }
}