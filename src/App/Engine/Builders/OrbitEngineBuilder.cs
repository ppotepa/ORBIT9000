<<<<<<< HEAD
﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ORBIT9000.Abstractions.Data;
using ORBIT9000.Abstractions.Providers;
using ORBIT9000.Abstractions.Runtime;
using ORBIT9000.Core.TempTools;
using ORBIT9000.Data;
using ORBIT9000.Data.Adapters;
using ORBIT9000.Data.Context;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Factories;
using ORBIT9000.Engine.IO.Loaders;
using ORBIT9000.Engine.IO.Loaders.Plugin;
using ORBIT9000.Engine.IO.Loaders.Plugin.Strategies;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;
using ORBIT9000.Engine.Providers;
using ORBIT9000.Engine.Runtime.State;
using ORBIT9000.Engine.Scheduling;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
=======
﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Loaders;
using System.Reflection;
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)

namespace ORBIT9000.Engine.Builders
{
    public class OrbitEngineBuilder
    {
<<<<<<< HEAD
<<<<<<< HEAD
        #region Fields

        private static readonly MethodInfo _createLoggerMethod = typeof(LoggerFactoryExtensions)
                    .GetMethod(nameof(ILoggerFactory.CreateLogger), [typeof(ILoggerFactory)])!;

        private static readonly ConcurrentDictionary<Type, Func<ILoggerFactory, object>> _loggerFactoryCache = new();
        private readonly ContainerBuilder _containerBuilder;
        private readonly ILogger<OrbitEngineBuilder>? _logger;
        private readonly ILoggerFactory _loggerFactory;
        private IConfiguration? _configuration;
        private RawEngineConfiguration? _rawConfiguration;

        #endregion Fields

        #region Constructors

        public OrbitEngineBuilder(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<OrbitEngineBuilder>();
            _containerBuilder = new ContainerBuilder();
        }

        public OrbitEngine Build()
        {
            _containerBuilder.RegisterInstance(_loggerFactory).As<ILoggerFactory>().SingleInstance();

            _ = _containerBuilder.RegisterGeneric((context, genericArguments, _) =>
            {
                Type categoryType = genericArguments[0];
                _logger!.LogDebug("Creating Logger for {Type}", categoryType.Name);

                ILoggerFactory loggerFactory = context.Resolve<ILoggerFactory>();

                Func<ILoggerFactory, object> factory = CreateLoggerFactory(categoryType);
                return factory(loggerFactory);
            })
            .As(typeof(ILogger<>)).InstancePerDependency();

            _containerBuilder.RegisterInstance(_configuration!).As<IConfiguration>().SingleInstance();
            _containerBuilder.RegisterInstance(_rawConfiguration!).AsSelf().SingleInstance();

            // Plugin loading
            _containerBuilder.RegisterType<StringArrayPluginLoader>().AsSelf().InstancePerDependency();
            _containerBuilder.RegisterType<DebugDirectoryPluginLoader>().AsSelf().InstancePerDependency();
            _containerBuilder.RegisterType<DirectoryPluginLoader>().AsSelf().InstancePerDependency();
            _containerBuilder.RegisterType<PluginLoaderFactory>().AsSelf().SingleInstance();

            _containerBuilder.Register(ctx =>
            {
                PluginLoaderFactory factory = ctx.Resolve<PluginLoaderFactory>();
                return factory.Create();
            })
            .As<IPluginLoader>()
            .SingleInstance();

            // Assembly loader
            _containerBuilder.RegisterType<AssemblyLoader>().As<IAssemblyLoader>().SingleInstance();

            // Runtime
            _containerBuilder.RegisterType<RuntimeSettings>().AsSelf().SingleInstance();
            _containerBuilder.RegisterType<PluginProvider>().As<IPluginProvider>().SingleInstance();
            _containerBuilder.RegisterGeneric(typeof(GlobalMessageChannel<>))
                                       .As(typeof(GlobalMessageChannel<>))
                                       .As(typeof(IMessageChannel<>))
                                       .SingleInstance();

            // Logger fallback
            _containerBuilder.Register(_ => _loggerFactory.CreateLogger<OrbitEngineBuilder>())
                                  .As<ILogger>()
                                  .SingleInstance();

            // Scheduling
            _containerBuilder.RegisterType<EngineState>().AsSelf().SingleInstance();
            _containerBuilder.RegisterType<ScheduleCalculator>().AsImplementedInterfaces().SingleInstance();
            _containerBuilder.RegisterType<SimpleScheduler>().AsImplementedInterfaces().SingleInstance();
            _containerBuilder.RegisterType<TextScheduleParser>().AsImplementedInterfaces().SingleInstance();

            // Core engine
            _containerBuilder.RegisterType<OrbitEngine>().AsSelf().SingleInstance();

            // .NET DI interop
            _containerBuilder.Register(c => new AutofacServiceProvider(c.Resolve<ILifetimeScope>()))
                                  .As<IServiceProvider>()
                                  .SingleInstance();

            _containerBuilder.RegisterType<InternalDbContextFactory>().AsSelf().SingleInstance();

            // WeatherData access layer
            _containerBuilder.RegisterType<ReflectiveInMemoryContext>().AsSelf().InstancePerDependency();
            _containerBuilder.RegisterType<ReflectiveInMemoryDbAdapter>().As<IDbAdapter>().InstancePerDependency();

            // WeatherData access layer - localdb
            _containerBuilder.RegisterType<LocalDbContext>().AsSelf().InstancePerDependency();
            _containerBuilder.RegisterType<LocalDbAdapter>().As<IDbAdapter>().InstancePerDependency();

            _containerBuilder.Register(ctx =>
            {
                InternalDbContextFactory factory = ctx.Resolve<InternalDbContextFactory>();
                return factory.ResolveContext();
            })
           .As<DbContext>()
           .InstancePerDependency();

            _containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerDependency();

            IContainer container = _containerBuilder.Build();
            return container.Resolve<OrbitEngine>();
        }

        /// <summary>
        /// Sets the configuration for the builder using an existing <see cref="IConfiguration"/> instance.
        /// This method is intended for scenarios where you want to inject a pre-built configuration,
        /// such as during integration tests or when composing configuration from multiple sources.
        /// </summary>
        public OrbitEngineBuilder Configure(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            return this;
        }

        /// <summary>
        /// Loads configuration from a JSON file or a <see cref="RawEngineConfiguration"/> object.
        /// These overloads are designed for quickly setting up the application runtime, especially
        /// for local development or rapid testing scenarios. They allow the engine to be configured
        /// with minimal setup, bypassing the need for a full configuration pipeline.
        /// </summary>
        public OrbitEngineBuilder UseConfiguration(string settingsPath = "appsettings.json")
        {
            if (_configuration is null)
            {
                if (!File.Exists(settingsPath))
                {
                    throw new FileNotFoundException($"Configuration file not found: {settingsPath}", settingsPath);
                }

                _configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(settingsPath, optional: false, reloadOnChange: false)
                    .Build();

                _rawConfiguration = _configuration.GetSection("OrbitEngine").Get<RawEngineConfiguration>()!;
            }
            else { throw new InvalidOperationException("Configuration has already been set."); }

            return this;
        }

        public OrbitEngineBuilder UseConfiguration(RawEngineConfiguration rawConfiguration)
        {
            string json = JsonConvert.SerializeObject(rawConfiguration);

            if (_configuration is null)
            {
                using MemoryStream stream = new(Encoding.UTF8.GetBytes(json));

                _configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonStream(stream)
                    .Build();
            }
            else { throw new InvalidOperationException("Configuration has already been set."); }

            _rawConfiguration = rawConfiguration;
            return this;
        }

        /// <summary>
        /// Creates a factory delegate for constructing generic ILogger&lt;T&gt; instances using the provided ILoggerFactory.
        /// Uses an Expression-based approach for efficient access.
        /// NOTE: Unable to get LoggerFactory.CreateLogger to work directly for generic types, so an Expression-based
        /// approach is used for faster access.
        /// TODO: Investigate and implement a more direct or idiomatic solution if possible.
        /// </summary>
        private static Func<ILoggerFactory, object> CreateLoggerFactory(Type categoryType)
        {
            return _loggerFactoryCache.GetOrAdd(categoryType, type =>
            {
                ParameterExpression factoryParam = Expression.Parameter(typeof(ILoggerFactory), "factory");
                MethodCallExpression callExpression = Expression.Call(null, _createLoggerMethod.MakeGenericMethod(type), factoryParam);

                Expression<Func<ILoggerFactory, object>> lambda = Expression.Lambda<Func<ILoggerFactory, object>>(
                    Expression.Convert(callExpression, typeof(object)),
                    factoryParam);

                return lambda.Compile();
            });
        }

        #endregion Constructors
=======
        public OrbitEngineBuilder()
        {
            UseSerilogLogging();
            this._logger = _loggerFactory?.CreateLogger<OrbitEngineBuilder>();  
        }

        private readonly string _outputTemplate =
            "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] [{SourceContext}]{Scope} {Message:lj}{NewLine}{Exception}";

=======
        private readonly ILogger<OrbitEngineBuilder>? _logger;
        private readonly Dictionary<Type, PluginRegistrationInfo> _plugins = new();
>>>>>>> 9aa9371 (Replace Serilog with Microsoft.Extensions.Logging)
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
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)
    }
}
