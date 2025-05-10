using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Providers;
using ORBIT9000.Core.Abstractions.Scheduling;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Runtime.State;
using ORBIT9000.Engine.Runtime.Strategies.Running;

namespace ORBIT9000.Engine
{
    public partial class OrbitEngine
    {
        #region Fields

        private readonly ILogger<OrbitEngine> _logger;
        private readonly Thread _mainThread;

        public IScheduler Scheduler { get; }

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

            this._logger = loggerFactory.CreateLogger<OrbitEngine>()
                ?? throw new InvalidOperationException("Logger could not be created.");

            this._mainThread = new Thread(Default.EngineStartupStrategy)
            {
                IsBackground = true,

                Name = "MainEngineThread"
            };

            this.PluginProvider = pluginProvider;
            this.ServiceProvider = serviceProvider;

            this.Scheduler = scheduler;

            this.IsInitialized = true;
            this.IsRunning = true;

            this.Configuration = configuration;
            this._logger.LogInformation("Engine initialized with configuration: {Configuration}", configuration);
        }

        #endregion Constructors

        #region Properties

        public bool IsInitialized { get; }
        public bool IsRunning { get; internal set; }
        public IPluginProvider PluginProvider { get; }
        public IServiceProvider ServiceProvider { get; }
        internal RuntimeSettings Configuration { get; set; }

        public void Start()
        {
            if (!this.IsInitialized)
            {
                throw new InvalidOperationException("Engine has not been initialized.");
            }

            ILifetimeScope root = this.ServiceProvider.GetAutofacRoot();
            EngineState state = root.Resolve<EngineState>();

            this._logger.LogInformation("Starting engine thread...");
            this._mainThread.Start(state);

            this.Scheduler.StartAsync();

            while (this.IsRunning)
            {
                Thread.Sleep(100);
            }
        }

        #endregion Methods
    }
}