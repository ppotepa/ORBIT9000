using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Exceptions;
using ORBIT9000.Engine.Loaders.Plugin.Results;
using ORBIT9000.Engine.State;
using Serilog;
using System.Reflection;

namespace ORBIT9000.Engine
{
    public class OrbitEngine
    {
        private const string OutputTemplate
           = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [{SourceContext}]{Scope} {Message:lj}{NewLine}{Exception}";

        private ExceptionFactory? _exceptionFactory;
        private ILogger<OrbitEngine>? _logger;
        private ILoggerFactory? _loggerFactory;
        private Thread? _mainThread;
        private Dictionary<Type, PluginActivationInfo> _plugins = new();
        private IConfiguration? _rawConfiguration;
        private IServiceProvider? _serviceProvider;
        private IServiceCollection? _servicesCollection;

        public OrbitEngine()
        {
            _servicesCollection = new ServiceCollection();
        }

        public OrbitEngineConfig? ConfigurationInfo { get; private set; }
        public bool IsInitialized { get; private set; }
        public bool IsRunning { get; private set; }

        public void Initialize()
        {
            if (IsInitialized)
            {
                _logger?.LogWarning("Engine is already initialized.");
                return;
            }

            SetupConfiguration();
            SetupLogging();
            RegisterCoreServices();

            ValidateConfiguration();
            var rawConfig = _rawConfiguration?.Get<RawOrbitEngineConfig>();
            RegisterPlugins(rawConfig);
            
            _serviceProvider = _servicesCollection?.BuildServiceProvider();
            _mainThread = new Thread(MainEngineThread);

            IsInitialized = true;
        }

        public void Start()
        {            
            if (!IsInitialized)
            {
                Initialize();
            }
            else if (IsRunning)
            {
                _logger?.LogWarning("Engine is already running.");
                return;
            }

            IsRunning = true;
            _mainThread?.Start(new EngineState { Engine = this });
        }

        private static void ProcessPlugins(OrbitEngine engine)
        {
            if (engine._serviceProvider == null)
                return;

            foreach (var pluginKey in engine._plugins)
            {
                Type pluginDescriptor = pluginKey.Key;

                engine._logger.LogInformation($"Plugin: {pluginKey} - {pluginDescriptor.Name}");

                using var scope = engine._serviceProvider.CreateAsyncScope();

                engine._logger.LogInformation($"Creating scope for {pluginKey} - {pluginDescriptor.Name}");

                if (scope.ServiceProvider.GetService(pluginDescriptor) is not IOrbitPlugin instance)
                {
                    engine._logger.LogWarning($"Plugin {pluginKey} - {pluginDescriptor.Name} could not be created.");
                    continue;
                }

                if (engine._plugins[pluginDescriptor].Instances.Any())
                {
                    engine._logger.LogWarning($"Plugin {pluginKey} - {pluginDescriptor.Name} is already running.");
                }
                else engine._plugins[pluginDescriptor].Instances.Add(Task.Run(() => instance.Run()));
            }
        }

        private void MainEngineThread(object? state)
        {
            if (state is not EngineState engineState || engineState.Engine is null)
            {
                throw new ArgumentNullException(nameof(state), "State was null or invalid.");
            }

            var engine = engineState.Engine;
            var logger = engine._logger;

            logger?.LogInformation("Engine is running.");

            while (engine.IsRunning)
            {
                ProcessPlugins(engine);
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        private void RegisterCoreServices()
        {
            if (_servicesCollection == null || _loggerFactory == null || _rawConfiguration == null)
                return;

            _servicesCollection.AddSingleton(_loggerFactory);
            _servicesCollection.AddLogging();
            _servicesCollection.AddSingleton(_rawConfiguration);
        }

        private void RegisterPlugins(RawOrbitEngineConfig? raw)
        {
            if (raw == null || _servicesCollection == null || _logger == null)
                return;

            _logger.LogInformation("Loading raw configuration.");

            ConfigurationInfo = OrbitEngineConfig.FromRaw(raw, _logger);

            if (ConfigurationInfo != null)
            {
                foreach (PluginLoadResult info in ConfigurationInfo.PluginInfo)
                {
                    foreach (Type item in info.Plugins)
                    {
                        _plugins.Add(item, new PluginActivationInfo(false));
                        _servicesCollection.AddScoped(item);
                    }
                }
            }

            foreach (var plugin in _plugins)
            {
                Type implementation = plugin.Key;

                if (!plugin.Value.Registered)
                {
                    var services = implementation.Assembly.GetTypes()
                        .Where(type => type.GetCustomAttribute<ServiceAttribute>() != null);

                    var dataProviders = implementation.Assembly.GetTypes()
                        .Where(type => type.GetCustomAttribute<DataProviderAttribute>() != null);

                    foreach (Type type in (Type[])[.. services, .. dataProviders])
                    {
                        _servicesCollection.AddScoped(type);
                    }
                }

                plugin.Value.Registered = true;
            }
        }

        private void SetupConfiguration()
        {
            _rawConfiguration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();
        }

        private void SetupLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo
                .Console(outputTemplate: OutputTemplate)
                .CreateLogger();

            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(Log.Logger);
            });

            _logger = _loggerFactory.CreateLogger<OrbitEngine>();
            _exceptionFactory = new ExceptionFactory(_logger, true);
        }

        private void ValidateConfiguration()
        {
            if (_rawConfiguration == null || !_rawConfiguration.AsEnumerable().Any())
            {
                _logger?.LogError("Configuration was NULL or EMPTY.");
                throw new InvalidOperationException("Configuration was NULL or EMPTY.");
            }
        }
    }
}
