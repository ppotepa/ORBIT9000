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
    public class OrbitEngineBuilder
    {
        private readonly string _outputTemplate =
            "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [{SourceContext}]{Scope} {Message:lj}{NewLine}{Exception}";

        private readonly Dictionary<Type, PluginActivationInfo> _plugins = [];
        private readonly IServiceCollection _services = new ServiceCollection();
        private IConfiguration? _configuration;
        private ILoggerFactory? _loggerFactory;

        public OrbitEngine Build()
        {
            if (_configuration == null || _loggerFactory == null)
            {
                throw new InvalidOperationException("Engine requires configuration and logging.");
            }
            
            ServiceProvider serviceProvider = _services.BuildServiceProvider();

            return new OrbitEngine(
                _configuration,
                _loggerFactory,
                serviceProvider,
                _plugins
            );
        }

        public OrbitEngineBuilder RegisterPluginDependencies()
        {
            foreach (KeyValuePair<Type, PluginActivationInfo> plugin in _plugins.Where(plugin => !plugin.Value.Registered))
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

        public OrbitEngineBuilder RegisterPlugins(Type[]? pluginTypes = null)
        {
            if (_configuration == null || _loggerFactory == null)
                throw new InvalidOperationException("Configuration and Logging must be configured before registering plugins.");

            // Load raw plugin configuration
            RawOrbitEngineConfig? rawPluginConfig = _configuration.Get<RawOrbitEngineConfig>();
            ILogger<OrbitEngineBuilder> logger = _loggerFactory.CreateLogger<OrbitEngineBuilder>();

            if (pluginTypes is null || pluginTypes.Length == 0)
            {
                if (rawPluginConfig == null)
                {
                    logger.LogError("Raw plugin configuration is missing.");
                    throw new InvalidOperationException("Raw plugin configuration is required to register plugins.");
                }

                OrbitEngineConfig? engineConfig = OrbitEngineConfig.FromRaw(rawPluginConfig, logger);

                if (engineConfig?.PluginInfo != null)
                {
                    pluginTypes = [.. engineConfig.PluginInfo.SelectMany(pluginLoadResult => pluginLoadResult.Plugins)];
                }
                else
                {
                    logger.LogWarning("No plugins found in the configuration.");
                    return this;
                }
            }

            foreach (var pluginType in pluginTypes)
            {
                // Check if the type implements IOrbitPlugin
                if (!typeof(IOrbitPlugin).IsAssignableFrom(pluginType))
                {
                    logger.LogWarning("Type {FullName} does not implement IOrbitPlugin and will be skipped.", pluginType.FullName);
                    continue;
                }

                if (!_plugins.ContainsKey(pluginType))
                {
                    _plugins.Add(pluginType, new PluginActivationInfo(false));
                    _services.AddScoped(pluginType);

                    logger.LogInformation("Registered plugin: {FullName}", pluginType.FullName);
                }
                else logger.LogWarning("Plugin {FullName} is already registered.", pluginType.FullName);
            }

            return this;
        }

        public OrbitEngineBuilder UseConfiguration(string settingsPath = "appsettings.json")
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(settingsPath, optional: false, reloadOnChange: false)
                .Build();

            _services.AddSingleton(_configuration);
            return this;
        }

        public OrbitEngineBuilder UseConfiguration(IConfiguration configuration)
        {
            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration));

            _services.AddSingleton(configuration);
            return this;
        }

        public OrbitEngineBuilder UseSerilogLogging()
        {
            if (_configuration == null)
                throw new InvalidOperationException("Configuration must be set before logging.");

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