using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Abstractions.Runtime;
using ORBIT9000.Core.Abstractions.Scheduling;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;
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

namespace ORBIT9000.Engine.Builders
{
    public class OrbitEngineBuilder
    {
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

        #endregion Constructors

        #region Methods

        public OrbitEngine Build()
        {
            _containerBuilder.RegisterInstance(_loggerFactory).As<ILoggerFactory>();

            _ = _containerBuilder.RegisterGeneric((context, genericArguments, parameters) =>
            {
                Type categoryType = genericArguments[0];
                _logger!.LogDebug("Creating Logger for {Type}", categoryType.Name);

                ILoggerFactory loggerFactory = context.Resolve<ILoggerFactory>();

                var factory = CreateLoggerFactory(categoryType);
                return factory(loggerFactory);
            })
            .As(typeof(ILogger<>)).InstancePerDependency();

            _containerBuilder.RegisterInstance(_configuration!).As<IConfiguration>().SingleInstance();
            _containerBuilder.RegisterInstance(_rawConfiguration!).AsSelf().SingleInstance();

            _containerBuilder.RegisterType<StringArrayPluginLoader>().AsSelf().InstancePerDependency();
            _containerBuilder.RegisterType<DebugDirectoryPluginLoader>().AsSelf().InstancePerDependency();
            _containerBuilder.RegisterType<DirectoryPluginLoader>().AsSelf().InstancePerDependency();
            _containerBuilder.RegisterType<PluginLoaderFactory>().AsSelf().InstancePerDependency();

            _containerBuilder.RegisterType<AssemblyLoader>().As<IAssemblyLoader>().SingleInstance();

            _containerBuilder.RegisterType<RuntimeSettings>().AsSelf().SingleInstance();
            _containerBuilder.RegisterType<PluginProvider>().As<IPluginProvider>().SingleInstance();
            _containerBuilder.RegisterGeneric(typeof(GlobalMessageChannel<>))
                               .As(typeof(GlobalMessageChannel<>))
                               .As(typeof(IMessageChannel<>))
                               .SingleInstance();

            _containerBuilder.Register(ctx => _loggerFactory.CreateLogger<OrbitEngineBuilder>()).As<ILogger>().SingleInstance();

            _containerBuilder.Register(ctx =>
            {
                PluginLoaderFactory factory = ctx.Resolve<PluginLoaderFactory>();
                return factory.Create();
            })
            .As<IPluginLoader>()
            .SingleInstance();

            _containerBuilder.RegisterType<OrbitEngine>()
                .AsSelf()
                .SingleInstance();

            _containerBuilder.RegisterType<EngineState>()
               .AsSelf()
            .SingleInstance();

            _containerBuilder.RegisterType<ScheduleCalculator>()
               .AsImplementedInterfaces()
               .SingleInstance();

            _containerBuilder.RegisterType<SimpleScheduler>()
                .AsImplementedInterfaces()
                .SingleInstance();

            _containerBuilder.Register(c => new AutofacServiceProvider(c.Resolve<ILifetimeScope>()))
                .As<IServiceProvider>()
                .SingleInstance();

            var container = _containerBuilder.Build();
            var engine = container.Resolve<OrbitEngine>();
            return engine;
        }

        public OrbitEngineBuilder Configure(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            return this;
        }

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
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

                _configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonStream(stream)
                    .Build();
            }
            else { throw new InvalidOperationException("Configuration has already been set."); }

            return this;
        }

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

        #endregion Methods
    }
}