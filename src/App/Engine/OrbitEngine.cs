using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Exceptions;
using ORBIT9000.Engine.Loaders;
using ORBIT9000.Engine.State;

namespace ORBIT9000.Engine
{
    public partial class OrbitEngine
    {
        private readonly InternalOrbitEngineConfig _configuration;
        private readonly ExceptionFactory _exceptionFactory;

        private readonly ILogger<OrbitEngine> _logger;
        private readonly Thread _mainThread;
        private readonly Dictionary<Type, PluginRegistrationInfo> _pluginRegistrations;
        private readonly IServiceProvider _serviceProvider;

        public OrbitEngine(
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider,
            InternalOrbitEngineConfig configuration,
            Dictionary<Type, PluginRegistrationInfo> pluginRegistrations
            )
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(loggerFactory);
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(pluginRegistrations);

            _logger = loggerFactory.CreateLogger<OrbitEngine>() ?? throw new InvalidOperationException("Logger could not be created.");
            _exceptionFactory = new ExceptionFactory(_logger, true);
            _mainThread = new Thread(Strategies.Running.Default.DefaultEngineStrategy);
            _pluginRegistrations = pluginRegistrations;
            _serviceProvider = serviceProvider;

            IsInitialized = true;
        }

        public bool IsInitialized { get; }
        public bool IsRunning { get; private set; }
        public Dictionary<Type, PluginRegistrationInfo> PluginRegistrations { get => _pluginRegistrations; }
        //public Dictionary<Type, PluginActivationInfo> PluginActivations { get => _pluginActivations; }
        public IServiceProvider ServiceProvider { get => _serviceProvider; }

        public void Start()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Engine has not been initialized.");

            if (IsRunning)
            {
                _logger?.LogWarning("Engine is already running.");
                return;
            }

            IsRunning = true;

            _mainThread.Start(new EngineState { Engine = this });
        }
    }
}