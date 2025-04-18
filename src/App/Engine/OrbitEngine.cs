using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;
using Serilog;

namespace ORBIT9000.Engine
{
    public class OrbitEngine
    {
        private const string OutputTemplate
           = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [{SourceContext}]{Scope} {Message:lj}{NewLine}{Exception}";

        private readonly ILogger<OrbitEngine> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _rawConfiguration;
        
        private readonly IServiceCollection _servicesCollection;
        private List<Type> _plugins = new();
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
                .CreateLogger()
                .ForContext<OrbitEngine>();
         
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(Log.Logger);
            });

            _logger = _loggerFactory.CreateLogger<OrbitEngine>();

            _servicesCollection.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(Log.Logger);
            });

            _servicesCollection.AddSingleton(_rawConfiguration);
        }
        
        public OrbitEngineConfig? ConfigurationInfo { get; private set; }
        
        public bool IsInitialized { get; private set; }

        public void Start()
        {
            if (IsInitialized is false)
            {
                Initialize();
            }
        }

        internal void Initialize()
        {
            if (_rawConfiguration is null || _rawConfiguration.AsEnumerable().Any() is false)
            {
                _logger.LogError("Configuration was NULL or EMPTY.");
                throw new InvalidOperationException("Configuration was NULL or EMPTY.");
            }

            RawOrbitEngineConfig? raw = _rawConfiguration.Get<RawOrbitEngineConfig>();

            if (raw is not null)
            {
                _logger.LogInformation("Loading raw configuration.");

                this.ConfigurationInfo = OrbitEngineConfig.FromRaw(raw, _logger);

                foreach (var info in this.ConfigurationInfo.PluginInfo)
                {
                    foreach (var item in info.Plugins)
                    {
                        _servicesCollection.AddScoped(item);
                    }
                }
            }

            _serviceProvider = _servicesCollection.BuildServiceProvider();
        }

    }
}