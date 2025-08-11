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
using Serilog;
using System.Reflection;
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)

namespace ORBIT9000.Engine.Builders
{
    public class OrbitEngineBuilder
    {
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
>>>>>>> e3e4b59 (Refactor Orbit Engine configuration and plugin loading)
    }
}