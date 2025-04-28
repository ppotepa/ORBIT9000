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
>>>>>>> 37a87d9 (Add Terminal AppSettings)
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
        private readonly ILogger<OrbitEngine> _logger;
        private readonly Thread _mainThread;
        private readonly IPluginProvider _pluginProvider;
        private readonly IServiceProvider _serviceProvider;
<<<<<<< HEAD
<<<<<<< HEAD
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)
=======
  
>>>>>>> 37a87d9 (Add Terminal AppSettings)
=======
>>>>>>> 2e9d040 (Add Basic Plugin Channel Handling)

        public OrbitEngine(
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider,
<<<<<<< HEAD
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
=======
            RuntimeConfiguration configuration,
>>>>>>> ed8e1ec (Remove PreBuild Helper)
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

            _logger = loggerFactory.CreateLogger<OrbitEngine>()
                ?? throw new InvalidOperationException("Logger could not be created.");

            _mainThread = new Thread(Strategies.Running.Default.EngineStartupStrategy);
            _mainThread.IsBackground = true;

            _mainThread.Name = "MainEngineThread";

            _pluginProvider = pluginProvider;
            _serviceProvider = serviceProvider;

            IsInitialized = true;
            _configuration = configuration;
            _logger.LogInformation("Engine initialized with configuration: {Configuration}", configuration);

        }

        public bool IsInitialized { get; }

        private RuntimeConfiguration _configuration;

        public bool IsRunning { get; private set; }
        public IPluginProvider PluginProvider { get => _pluginProvider; }
        public IServiceProvider ServiceProvider { get => _serviceProvider; }
        internal RuntimeConfiguration Configuration { get => _configuration; set => _configuration = value; }
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

            _mainThread.Start(_serviceProvider.GetAutofacRoot().Resolve<EngineState>());
        }
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)
    }
}