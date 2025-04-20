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
        private const string OutputTemplate =
            "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [{SourceContext}]{Scope} {Message:lj}{NewLine}{Exception}";

        private readonly OrbitEngineConfig _configuration;
        private readonly ExceptionFactory _exceptionFactory;
        private readonly ILogger<OrbitEngine> _logger;
        private readonly Thread _mainThread;
        private readonly IReadOnlyDictionary<Type, PluginActivationInfo> _plugins;
        private readonly IServiceProvider _serviceProvider;

        internal OrbitEngine(
            IConfiguration rawConfiguration,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider,
            IReadOnlyDictionary<Type, PluginActivationInfo> plugins)
        {
            ArgumentNullException.ThrowIfNull(rawConfiguration, nameof(rawConfiguration));
            ArgumentNullException.ThrowIfNull(loggerFactory, nameof(loggerFactory));
            ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));
            ArgumentNullException.ThrowIfNull(plugins, nameof(plugins));

            _logger = loggerFactory.CreateLogger<OrbitEngine>() ?? throw new InvalidOperationException("Logger could not be created.");
            _configuration = OrbitEngineConfig.FromRaw(rawConfiguration.Get<RawOrbitEngineConfig>(), _logger)
                ?? throw new InvalidOperationException("Configuration could not be created.");
            _exceptionFactory = new ExceptionFactory(_logger, true);
            _mainThread = new Thread(MainEngineThread);
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

        private static void ProcessPlugins(OrbitEngine engine)
        {
            foreach (var (type, pluginInfo) in engine._plugins)
            {
                engine._logger.LogInformation($"Plugin: {type.Name}");

                using var scope = engine._serviceProvider.CreateAsyncScope();
                engine._logger.LogInformation($"Creating scope for {type.Name}");

                if (!TryGetPluginInstance(scope.ServiceProvider, type, engine._logger, out var instance))
                {
                    continue;
                }

                if (pluginInfo.Instances.Any() && !pluginInfo.AllowMultiple)
                {
                    engine._logger.LogWarning($"Plugin {type.Name} is already running.");
                    continue;
                }

                StartPluginTask(engine, type, pluginInfo);
            }
        }

        private static void StartPluginTask(OrbitEngine engine, Type type, PluginActivationInfo pluginInfo)
        {
            Task task = Task.Run(async () =>
            {
                await using var scope = engine._serviceProvider.CreateAsyncScope();
                if (!TryGetPluginInstance(scope.ServiceProvider, type, engine._logger, out var instance))
                {
                    return;
                }

                await instance.Run();
            });

            pluginInfo.Instances.Add(task);

            task.ContinueWith(completed =>
            {
                engine._logger.LogInformation($"Plugin {type.Name} has finished running.");
                pluginInfo.Instances.Remove(completed);
            });
        }

        private static bool TryGetPluginInstance(IServiceProvider serviceProvider, Type type, ILogger logger, out IOrbitPlugin? instance)
        {
            instance = serviceProvider.GetRequiredService(type) as IOrbitPlugin;

            if (instance == null)
            {
                logger.LogWarning($"Plugin {type.Name} could not be created.");
                return false;
            }

            return true;
        }
        private void MainEngineThread(object? state)
        {
            _logger.LogInformation("Engine is running.");

            while (IsRunning)
            {
                ProcessPlugins(this);
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}