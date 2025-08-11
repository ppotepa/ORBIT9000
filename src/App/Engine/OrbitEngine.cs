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
<<<<<<< HEAD
>>>>>>> 6133b24 (Refactor plugin architecture and assembly loading)
=======
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Loaders;
using ORBIT9000.Engine.Runtime.Exceptions;
using ORBIT9000.Engine.State;
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)

namespace ORBIT9000.Engine
{
    public partial class OrbitEngine
    {
<<<<<<< HEAD
<<<<<<< HEAD
        #region Fields

        private readonly ILogger<OrbitEngine> _logger;
        private readonly Thread _mainThread;

        public IScheduler Scheduler { get; }

        #endregion Fields

        #region Constructors
=======
        private readonly InternalOrbitEngineConfig _configuration;
=======
        private readonly InitializedInternalConfig _configuration;
>>>>>>> a1c6c63 (Refactor plugin architecture and configuration handling)
        private readonly ExceptionFactory _exceptionFactory;

        private readonly ILogger<OrbitEngine> _logger;
        private readonly Thread _mainThread;
        private readonly IPluginProvider _pluginProvider;
        private readonly IServiceProvider _serviceProvider;
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)

        public OrbitEngine(
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider,
<<<<<<< HEAD
<<<<<<< HEAD
            RuntimeSettings configuration,
            IPluginProvider pluginProvider,
            IScheduler scheduler
=======
            InternalOrbitEngineConfig configuration,
            Dictionary<Type, PluginRegistrationInfo> pluginRegistrations
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)
=======
            InitializedInternalConfig configuration,
            IPluginProvider pluginProvider
>>>>>>> a1c6c63 (Refactor plugin architecture and configuration handling)
            )
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(loggerFactory);
            ArgumentNullException.ThrowIfNull(serviceProvider);
<<<<<<< HEAD
<<<<<<< HEAD
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
=======
            ArgumentNullException.ThrowIfNull(pluginRegistrations);
=======
            ArgumentNullException.ThrowIfNull(pluginProvider);
>>>>>>> a1c6c63 (Refactor plugin architecture and configuration handling)

            _logger = loggerFactory.CreateLogger<OrbitEngine>() ?? throw new InvalidOperationException("Logger could not be created.");
            _exceptionFactory = new ExceptionFactory(_logger, true);
            _mainThread = new Thread(Strategies.Running.Default.EngineStartupStrategy);
            _pluginProvider = pluginProvider;
            _serviceProvider = serviceProvider;

            IsInitialized = true;
            _logger.LogInformation("Engine initialized with configuration: {Configuration}", configuration); 
        }

        public bool IsInitialized { get; }
        public bool IsRunning { get; private set; }
        public IPluginProvider PluginProvider { get => _pluginProvider; }

        //public Dictionary<Type, PluginActivationInfo> PluginActivations { get => _pluginActivations; }
        public IServiceProvider ServiceProvider { get => _serviceProvider; }

        public void Start()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Engine has not been initialized.");

            if (IsRunning)
            {
                _logger.LogWarning("Engine is already running.");
                return;
            }

            IsRunning = true;

            _mainThread.Start(new EngineState { Engine = this });
        }
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)
    }
}