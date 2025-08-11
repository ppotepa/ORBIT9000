<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions.Providers;
using ORBIT9000.Abstractions.Scheduling;
=======
﻿using Microsoft.Extensions.Configuration;
=======
﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
<<<<<<< HEAD
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
>>>>>>> 2e9d040 (Add Basic Plugin Channel Handling)
=======
>>>>>>> 56ba6c0 (Add Generic Message Channel)
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions;
<<<<<<< HEAD
>>>>>>> 37a87d9 (Add Terminal AppSettings)
=======
using ORBIT9000.Core.Abstractions.Scheduling;
>>>>>>> a7c6658 (Add Very Basic Job Scheduling)
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Runtime.State;
using ORBIT9000.Engine.Runtime.Strategies.Running;
=======
﻿using Microsoft.Extensions.Logging;
<<<<<<< HEAD
<<<<<<< HEAD
>>>>>>> 6133b24 (Refactor plugin architecture and assembly loading)
=======
=======
using ORBIT9000.Abstractions;
>>>>>>> 254394d (Remove OverLogging)
using ORBIT9000.Engine.Configuration;
<<<<<<< HEAD
using ORBIT9000.Engine.Runtime.Exceptions;
<<<<<<< HEAD
using ORBIT9000.Engine.State;
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)
=======
=======
>>>>>>> 590e002 (Add Temporary NamedPipe and Receiving Console App)
using ORBIT9000.Engine.Runtime.State;
>>>>>>> 254394d (Remove OverLogging)

namespace ORBIT9000.Engine
{
    public partial class OrbitEngine
    {
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
        #region Fields

=======
>>>>>>> 590e002 (Add Temporary NamedPipe and Receiving Console App)
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
=======
        private readonly RuntimeConfiguration _configuration;
>>>>>>> ed8e1ec (Remove PreBuild Helper)
        private readonly ExceptionFactory _exceptionFactory;
=======
>>>>>>> 53c6dc2 (Further Remove code smells.)

=======
>>>>>>> 15b0cd8 (Remove unused fields)
=======
        private readonly ExceptionFactory _exceptionFactory;

>>>>>>> e1e815e (Add Plugin Activation Checks)
=======
        #region Fields

>>>>>>> 18f5855 (Replace Dictionary of Actions with more clean BindingAction Type)
        private readonly ILogger<OrbitEngine> _logger;
        private readonly Thread _mainThread;
        private readonly IPluginProvider _pluginProvider;
        private readonly IServiceProvider _serviceProvider;
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)
=======
  
>>>>>>> 37a87d9 (Add Terminal AppSettings)
=======
>>>>>>> 2e9d040 (Add Basic Plugin Channel Handling)
=======
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
>>>>>>> a7c6658 (Add Very Basic Job Scheduling)

        private RuntimeSettings _configuration;

        #endregion Fields

        #region Constructors

        public OrbitEngine(
                    ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider,
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
            RuntimeSettings configuration,
            IPluginProvider pluginProvider,
            IScheduler scheduler
<<<<<<< HEAD
=======
            InternalOrbitEngineConfig configuration,
            Dictionary<Type, PluginRegistrationInfo> pluginRegistrations
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)
=======
            InitializedInternalConfig configuration,
=======
            RuntimeConfiguration configuration,
>>>>>>> ed8e1ec (Remove PreBuild Helper)
=======
            RuntimeSettings configuration,
>>>>>>> 72c40c3 (Add Basic Event Handling for Settings)
            IPluginProvider pluginProvider
>>>>>>> a1c6c63 (Refactor plugin architecture and configuration handling)
=======
>>>>>>> a7c6658 (Add Very Basic Job Scheduling)
            )
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(loggerFactory);
            ArgumentNullException.ThrowIfNull(serviceProvider);
<<<<<<< HEAD
<<<<<<< HEAD
            ArgumentNullException.ThrowIfNull(pluginProvider);
            ArgumentNullException.ThrowIfNull(scheduler);
<<<<<<< HEAD

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
=======
>>>>>>> a7c6658 (Add Very Basic Job Scheduling)

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
<<<<<<< HEAD
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)
=======

        #endregion Methods
>>>>>>> 18f5855 (Replace Dictionary of Actions with more clean BindingAction Type)
    }
}