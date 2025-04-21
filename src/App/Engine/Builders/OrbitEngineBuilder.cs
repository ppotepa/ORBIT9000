using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Loaders;
using Serilog;
using System.Reflection;

namespace ORBIT9000.Engine.Builders
{
    public class OrbitEngineBuilder
    {
        public OrbitEngineBuilder()
        {
            UseSerilogLogging();
            this._logger = _loggerFactory?.CreateLogger<OrbitEngineBuilder>();  
        }

        private readonly string _outputTemplate =
            "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [{SourceContext}]{Scope} {Message:lj}{NewLine}{Exception}";

        private readonly IServiceCollection _services = new ServiceCollection();
        private IConfiguration? _configuration;
        private InternalOrbitEngineConfig? _internalOrbitEngineConfig;
        private ILogger<OrbitEngineBuilder>? _logger;
        private ILoggerFactory? _loggerFactory;
        private Dictionary<Type, PluginRegistrationInfo> _plugins = new();

        public OrbitEngine Build()
        {
            if (_loggerFactory == null)
            {
                _loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.ClearProviders();
                });
            }

            if (_configuration is null)
            {
                Dictionary<string, string> initialData = new();

                _configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(initialData)
                    .Build();
            }

            _services.AddSingleton(_internalOrbitEngineConfig!);
            _services.AddSingleton<OrbitEngine>();

            RegisterPluginDependencies();

            return _services.BuildServiceProvider().GetRequiredService<OrbitEngine>();
        }

        public OrbitEngineBuilder RegisterPlugins(params Type[] types)
        {
            if (_configuration == null || _loggerFactory == null)
                throw new InvalidOperationException("Configuration and Logging must be configured before registering plugins.");

            ILogger<OrbitEngineBuilder> logger = _loggerFactory.CreateLogger<OrbitEngineBuilder>();

            if (types is not null && types.Length != 0)
            {
                foreach (var pluginType in types)
                {
                    // Check if the type implements IOrbitPlugin
                    if (!typeof(IOrbitPlugin).IsAssignableFrom(pluginType))
                    {
                        logger.LogWarning("Type {FullName} does not implement IOrbitPlugin and will be skipped.", pluginType.FullName);
                        continue;
                    }

                    if (!_plugins.ContainsKey(pluginType))
                    {
                        _plugins.Add(pluginType, new PluginRegistrationInfo(false));
                        _services.AddScoped(pluginType);
                        logger.LogInformation("Registered plugin: {FullName}", pluginType.FullName);
                    }
                    else logger.LogWarning("Plugin {FullName} is already registered.", pluginType.FullName);
                }
            }
            else
            {
                _plugins = _internalOrbitEngineConfig!.PluginInfo
                    .SelectMany(plugin => plugin.Plugins)
                    .Distinct()
                    .ToDictionary(plugin => plugin, plugin => new PluginRegistrationInfo(false));
            }

            _services.AddSingleton(_plugins);
            return this;
        }

        public OrbitEngineBuilder RegisterPluginDependencies()
        {
            foreach (KeyValuePair<Type, PluginRegistrationInfo> plugin in _plugins.Where(plugin => !plugin.Value.Registered))
            {
                Assembly assembly = plugin.Key.Assembly;

                IEnumerable<Type> serviceTypes = assembly.GetTypes()
                    .Where(type => type.GetCustomAttribute<ServiceAttribute>() != null);

                IEnumerable<Type> providerTypes = assembly.GetTypes()
                    .Where(type => type.GetCustomAttribute<DataProviderAttribute>() != null);

                foreach (Type? type in serviceTypes.Concat(providerTypes))
                    _services.AddScoped(type);

                plugin.Value.Registered = true;
            }

            return this;
        }

        public OrbitEngineBuilder UseConfiguration(string settingsPath = "appsettings.json")
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(settingsPath, optional: false, reloadOnChange: false)
                .Build();

            OrbitEngineConfiguration? config = _configuration.Get<OrbitEngineConfiguration>();
            _internalOrbitEngineConfig = InternalOrbitEngineConfig.FromRaw(config!);

            _services.AddSingleton(_configuration);
            return this;
        }

        public OrbitEngineBuilder UseConfiguration(IConfiguration configuration)
        {
            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration));

            OrbitEngineConfiguration? config = configuration.Get<OrbitEngineConfiguration>();
            _internalOrbitEngineConfig = InternalOrbitEngineConfig.FromRaw(config!);

            _services.AddSingleton(configuration);
            return this;
        }

        public OrbitEngineBuilder UseConfiguration(InternalOrbitEngineConfig configuration)
        {
            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration));

            _internalOrbitEngineConfig = configuration;

            _services.AddSingleton(configuration);
            return this;
        }

        public OrbitEngineBuilder UseSerilogLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: _outputTemplate)
                .CreateLogger();

            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(Log.Logger);
            });

            _services.AddSingleton(_loggerFactory);
            _services.AddLogging();

            return this;
        }
    }
}