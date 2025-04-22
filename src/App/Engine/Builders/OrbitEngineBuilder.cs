using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Loaders;
using System.Reflection;

namespace ORBIT9000.Engine.Builders
{
    public class OrbitEngineBuilder
    {
        private readonly ILogger<OrbitEngineBuilder>? _logger;
        private readonly Dictionary<Type, PluginRegistrationInfo> _plugins = new();
        private readonly IServiceCollection _services = new ServiceCollection();
        private IConfiguration? _configuration;
        private InternalOrbitEngineConfig? _internalOrbitEngineConfig;
        private ILoggerFactory _loggerFactory;

        public OrbitEngineBuilder(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<OrbitEngineBuilder>();
            _logger.LogDebug("OrbitEngineBuilder initialized");
        }

        public OrbitEngine Build()
        {
            _logger?.LogInformation("Building OrbitEngine...");

            EnsureLoggerFactoryInitialized();
            EnsureConfigurationInitialized();

            _services.AddSingleton(_internalOrbitEngineConfig!);
            _services.AddSingleton<ILoggerFactory>(_loggerFactory);
            _services.AddSingleton<OrbitEngine>();

            _logger?.LogDebug("Registering plugin dependencies");
            RegisterPluginDependencies();

            _logger?.LogDebug("Building service provider");
            var serviceProvider = _services.BuildServiceProvider();
            var engine = serviceProvider.GetRequiredService<OrbitEngine>();
            _logger?.LogInformation("OrbitEngine successfully built");

            return engine;
        }

        public OrbitEngineBuilder RegisterPluginDependencies()
        {
            _logger?.LogInformation("Registering dependencies for {PluginCount} plugin(s)",
                _plugins.Count(p => !p.Value.Registered));

            foreach (var (pluginType, registrationInfo) in _plugins.Where(p => !p.Value.Registered))
            {
                _logger?.LogDebug("Registering dependencies from assembly {Assembly}", pluginType.Assembly.GetName().Name);
                RegisterDependenciesFromAssembly(pluginType.Assembly);
                registrationInfo.Registered = true;
                _logger?.LogTrace("Successfully registered dependencies for plugin {PluginType}", pluginType.Name);
            }

            return this;
        }

        public OrbitEngineBuilder RegisterPlugins(params Type[] types)
        {
            _logger?.LogInformation("Registering plugins...");
            EnsureConfigurationAndLoggingInitialized();

            if (types is { Length: > 0 })
            {
                _logger?.LogDebug("Registering {Count} explicitly provided plugin types", types.Length);
                foreach (var pluginType in types)
                {
                    RegisterPluginType(pluginType);
                }
            }
            else
            {
                _logger?.LogDebug("No plugin types explicitly provided, loading from configuration");
                RegisterPluginsFromConfig();
            }

            _services.AddSingleton(_plugins);
            _logger?.LogInformation("Successfully registered {Count} plugin(s)", _plugins.Count);
            return this;
        }

        public OrbitEngineBuilder UseConfiguration(string settingsPath = "appsettings.json")
        {
            _logger?.LogInformation("Loading configuration from file: {Path}", settingsPath);

            try
            {
                _configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(settingsPath, optional: false, reloadOnChange: false)
                    .Build();

                LoadInternalConfigFromConfiguration();
                _services.AddSingleton(_configuration);
                _logger?.LogInformation("Configuration successfully loaded from {Path}", settingsPath);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to load configuration from {Path}", settingsPath);
                throw;
            }

            return this;
        }

        public OrbitEngineBuilder UseConfiguration(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            _logger?.LogInformation("Using provided configuration instance");

            _configuration = configuration;
            LoadInternalConfigFromConfiguration();
            _services.AddSingleton(configuration);

            return this;
        }

        public OrbitEngineBuilder UseConfiguration(InternalOrbitEngineConfig configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            _logger?.LogInformation("Using provided internal configuration instance");

            _internalOrbitEngineConfig = configuration;
            _services.AddSingleton(configuration);

            return this;
        }

        private void EnsureConfigurationAndLoggingInitialized()
        {
            if (_configuration == null || _loggerFactory == null)
            {
                _logger?.LogError("Configuration or logging factory not initialized");
                throw new InvalidOperationException("Configuration and Logging must be configured before registering plugins.");
            }
        }

        private void EnsureConfigurationInitialized()
        {
            if (_configuration == null)
            {
                _logger?.LogDebug("No configuration provided, creating default empty configuration");
                _configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection([])
                    .Build();
            }
        }

        private void EnsureLoggerFactoryInitialized()
        {
            if (_loggerFactory == null)
            {
                _logger?.LogDebug("No logger factory provided, creating default logger factory");
                _loggerFactory = LoggerFactory.Create(builder => builder.ClearProviders());
                _services.AddSingleton(_loggerFactory);
            }
        }

        private void LoadInternalConfigFromConfiguration()
        {
            if (_configuration == null)
            {
                _logger?.LogError("Cannot load internal configuration: configuration is null");
                throw new InvalidOperationException("Configuration must be initialized before loading internal configuration.");
            }

            _logger?.LogDebug("Creating internal engine configuration from raw configuration");
            var config = _configuration.Get<OrbitEngineConfiguration>();
            if (config == null)
            {
                _logger?.LogWarning("Raw configuration could not be deserialized to OrbitEngineConfiguration");
            }

            _internalOrbitEngineConfig = InternalOrbitEngineConfig.FromRaw(config!);
            _logger?.LogDebug("Internal engine configuration created successfully");
        }

        private void RegisterDependenciesFromAssembly(Assembly assembly)
        {
            try
            {
                _logger?.LogTrace("Scanning assembly {Assembly} for service and provider types", assembly.GetName().Name);

                var serviceTypes = assembly.GetTypes()
                    .Where(type => type.GetCustomAttribute<ServiceAttribute>() != null);

                var providerTypes = assembly.GetTypes()
                    .Where(type => type.GetCustomAttribute<DataProviderAttribute>() != null);

                var allTypes = serviceTypes.Concat(providerTypes).ToList();
                _logger?.LogTrace("Found {Count} service/provider types in assembly {Assembly}",
                    allTypes.Count, assembly.GetName().Name);

                foreach (var type in allTypes)
                {
                    _services.AddScoped(type);
                    _logger?.LogTrace("Registered dependency: {Type}", type.FullName);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error while registering dependencies from assembly {Assembly}",
                    assembly.GetName().Name);
                throw;
            }
        }

        private void RegisterPluginsFromConfig()
        {
            if (_internalOrbitEngineConfig == null)
            {
                _logger?.LogError("Cannot register plugins from config: internal configuration is null");
                throw new InvalidOperationException("Internal configuration must be initialized before registering plugins.");
            }

            _logger?.LogDebug("Loading plugins from configuration");
            var pluginCount = 0;

            foreach (var pluginType in _internalOrbitEngineConfig.PluginInfo
                .SelectMany(plugin => plugin.Plugins)
                .Distinct())
            {
                if (_plugins.TryAdd(pluginType, new PluginRegistrationInfo(false)))
                {
                    _services.AddScoped(pluginType);
                    pluginCount++;
                    _logger?.LogTrace("Added plugin from config: {PluginType}", pluginType.FullName);
                }
            }

            _logger?.LogInformation("Loaded {Count} plugins from configuration", pluginCount);
        }

        private void RegisterPluginType(Type pluginType)
        {
            if (!typeof(IOrbitPlugin).IsAssignableFrom(pluginType))
            {
                _logger?.LogWarning("Type {FullName} does not implement IOrbitPlugin and will be skipped", pluginType.FullName);
                return;
            }

            if (_plugins.ContainsKey(pluginType))
            {
                _logger?.LogWarning("Plugin {FullName} is already registered", pluginType.FullName);
                return;
            }

            _plugins.Add(pluginType, new PluginRegistrationInfo(false));
            _services.AddScoped(pluginType);
            _logger?.LogInformation("Registered plugin: {FullName}", pluginType.FullName);
        }
    }
}
