using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;
using Serilog;
using System.Reflection;

namespace ORBIT9000.Engine
{

    class EngineState
    {
        public OrbitEngine Engine { get; internal set; }
    }

    public class OrbitEngine
    {
        private const string OutputTemplate
           = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [{SourceContext}]{Scope} {Message:lj}{NewLine}{Exception}";

        private readonly Thread MainThread = new Thread(MainEngineThread);

        private static void MainEngineThread(object? obj)
        {
            EngineState? state = obj as EngineState;

            if (state is null)
            {
                ArgumentNullException.ThrowIfNull(state, nameof(state));
            }
            while (state.Engine.IsRunning)
            {
                state.Engine._logger.LogInformation("Engine is running.");
                
                List<IOrbitPlugin?> instances = [.. state.Engine._plugins.Select(
                 (                 
                     plugin => state.Engine._serviceProvider.GetService(plugin.Value.Item) as IOrbitPlugin
                 ))];

                instances.ForEach(p => p.Run());

                Thread.Sleep(100);
            }
        }

        private readonly ILogger<OrbitEngine> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _rawConfiguration;
        private readonly IServiceCollection _servicesCollection;

        private Dictionary<string, PluginActivationInfo> _plugins = new();

        private IServiceProvider _serviceProvider;
        public OrbitEngine()
        {
            _servicesCollection = new ServiceCollection();

            _rawConfiguration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

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

            _servicesCollection.AddSingleton(this._loggerFactory);
            _servicesCollection.AddLogging();
            _servicesCollection.AddSingleton(_rawConfiguration);

            _logger = _loggerFactory.CreateLogger<OrbitEngine>();
        }

        public OrbitEngineConfig? ConfigurationInfo { get; private set; }

        public bool IsInitialized { get; private set; }
        public bool IsRunning { get; private set; }

        public void Start()
        {
            if (IsInitialized is false)
            {
                Initialize();
                MainThread.Start(new EngineState() { Engine = this });
            }
            else _logger.LogWarning("Engine is already initialized.");
        }

        internal void Initialize()
        {
            if (_rawConfiguration is null || _rawConfiguration.AsEnumerable().Any() is false)
            {
                _logger.LogError("Configuration was NULL or EMPTY.");
                throw new InvalidOperationException("Configuration was NULL or EMPTY.");
            }

            RawOrbitEngineConfig? raw = _rawConfiguration.Get<RawOrbitEngineConfig>();

            this.RegisterPlugins(raw);

            _serviceProvider = _servicesCollection.BuildServiceProvider();

            IsInitialized = true;
            IsRunning = true;
        }

        private void RegisterPlugins(RawOrbitEngineConfig? raw)
        {
            if (raw is not null)
            {
                _logger.LogInformation("Loading raw configuration.");

                this.ConfigurationInfo = OrbitEngineConfig.FromRaw(raw, _logger);

                foreach (Loaders.Plugin.Results.PluginLoadResult info in this.ConfigurationInfo?.PluginInfo)
                {
                    foreach (Type item in info.Plugins)
                    {
                        _plugins.Add(item.Name, new PluginActivationInfo(false, item));
                        _servicesCollection.AddScoped(item);
                    }
                }
            }

            /// temporary service provider that will be replaced later on
            /// used only for Initialization purposes            
            _serviceProvider = _servicesCollection.BuildServiceProvider();

            foreach (KeyValuePair<string, PluginActivationInfo> plugin in _plugins)
            {
                Type implementation = plugin.Value.Item;

                if (plugin.Value.Registered is false)
                {
                    IEnumerable<Type> services = implementation.Assembly.GetTypes().Where(type => type.GetCustomAttribute<ServiceAttribute>() != null);
                    IEnumerable<Type> dataProvider = implementation.Assembly.GetTypes().Where(type => type.GetCustomAttribute<DataProviderAttribute>() != null);

                    foreach (Type type in (Type[])[.. services, .. dataProvider])
                    {
                        _servicesCollection.AddScoped(type);
                    }
                }

                plugin.Value.Registered = true;

            }
        }
    }
}