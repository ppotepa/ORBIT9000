<<<<<<< HEAD
﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions.Providers;
using ORBIT9000.Abstractions.Scheduling;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Runtime.State;
using ORBIT9000.Engine.Runtime.Strategies.Running;
=======
﻿using Microsoft.Extensions.Logging;
>>>>>>> 6133b24 (Refactor plugin architecture and assembly loading)

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

            _logger = loggerFactory.CreateLogger<OrbitEngine>()
                ?? throw new InvalidOperationException("Logger could not be created.");

            _mainThread = new Thread(Default.EngineStartupStrategy)
            {
                IsBackground = true,

                Name = "MainEngineThread"
            };

            PluginProvider = pluginProvider;
            ServiceProvider = serviceProvider;

            Scheduler = scheduler;

            IsInitialized = true;
            IsRunning = true;

            Configuration = configuration;
            _logger.LogInformation("Engine initialized with configuration: {Configuration}", configuration);
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
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Engine has not been initialized.");
            }

            ILifetimeScope root = ServiceProvider.GetAutofacRoot();
            EngineState state = root.Resolve<EngineState>();

            _logger.LogInformation("Starting engine thread...");
            _mainThread.Start(state);

            Scheduler.StartAsync();

            while (IsRunning)
            {
                Thread.Sleep(100);
            }
        }

        #endregion Methods
    }
}