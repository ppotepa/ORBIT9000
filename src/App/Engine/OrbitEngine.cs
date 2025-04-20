using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Exceptions;
using ORBIT9000.Engine.State;

namespace ORBIT9000.Engine
{
    public class OrbitEngine
    {
        internal readonly OrbitEngineConfig _configuration;
        internal readonly ExceptionFactory _exceptionFactory;
        internal readonly ILogger<OrbitEngine> _logger;
        internal readonly Thread _mainThread;
        internal readonly IReadOnlyDictionary<Type, PluginActivationInfo> _plugins;
        internal readonly IServiceProvider _serviceProvider;

        internal OrbitEngine(
            IConfiguration rawConfiguration,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider,
            IReadOnlyDictionary<Type, PluginActivationInfo> plugins)
        {
            ArgumentNullException.ThrowIfNull(rawConfiguration);
            ArgumentNullException.ThrowIfNull(loggerFactory);
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(plugins);

            RawOrbitEngineConfig? boundConfig = rawConfiguration.Get<RawOrbitEngineConfig>();

            _logger = loggerFactory.CreateLogger<OrbitEngine>() ?? throw new InvalidOperationException("Logger could not be created.");
            _configuration = OrbitEngineConfig.FromRaw(boundConfig, _logger)
                ?? throw new InvalidOperationException("Configuration could not be created.");

            _exceptionFactory = new ExceptionFactory(_logger, true);
            _mainThread = new Thread(Strategies.Running.Default.DefaultEngineStrategy);
            _plugins = new Dictionary<Type, PluginActivationInfo>(plugins);
            _serviceProvider = serviceProvider;
            IsInitialized = true;
        }

        public bool IsInitialized { get; }
        public bool IsRunning { get; private set; }

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