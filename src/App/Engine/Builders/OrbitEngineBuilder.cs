using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ORBIT9000.Core.Abstractions.Data;
using ORBIT9000.Core.Abstractions.Providers;
using ORBIT9000.Core.Abstractions.Runtime;
using ORBIT9000.Core.TempTools;
using ORBIT9000.Data;
using ORBIT9000.Data.Adapters;
using ORBIT9000.Data.Context.ORBIT9000.Data.Context;
using ORBIT9000.Data.ORBIT9000.Data.Context;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;
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
            this._loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this._logger = this._loggerFactory.CreateLogger<OrbitEngineBuilder>();
            this._containerBuilder = new ContainerBuilder();
        }

        #endregion Constructors

        #region Methods

        public OrbitEngine Build()
        {
            this._containerBuilder.RegisterInstance(this._loggerFactory).As<ILoggerFactory>();

            _ = this._containerBuilder.RegisterGeneric((context, genericArguments, _) =>
            {
                Type categoryType = genericArguments[0];
                this._logger!.LogDebug("Creating Logger for {Type}", categoryType.Name);

                ILoggerFactory loggerFactory = context.Resolve<ILoggerFactory>();

                Func<ILoggerFactory, object> factory = CreateLoggerFactory(categoryType);
                return factory(loggerFactory);
            })
            .As(typeof(ILogger<>)).InstancePerDependency();

            this._containerBuilder.RegisterInstance(this._configuration!).As<IConfiguration>().SingleInstance();
            this._containerBuilder.RegisterInstance(this._rawConfiguration!).AsSelf().SingleInstance();

            // Plugin loading
            this._containerBuilder.RegisterType<StringArrayPluginLoader>().AsSelf().InstancePerDependency();
            this._containerBuilder.RegisterType<DebugDirectoryPluginLoader>().AsSelf().InstancePerDependency();
            this._containerBuilder.RegisterType<DirectoryPluginLoader>().AsSelf().InstancePerDependency();
            this._containerBuilder.RegisterType<PluginLoaderFactory>().AsSelf().InstancePerDependency();

            this._containerBuilder.Register(ctx =>
            {
                PluginLoaderFactory factory = ctx.Resolve<PluginLoaderFactory>();
                return factory.Create();
            })
            .As<IPluginLoader>()
            .SingleInstance();

            // Assembly loader
            this._containerBuilder.RegisterType<AssemblyLoader>().As<IAssemblyLoader>().SingleInstance();

            // Runtime
            this._containerBuilder.RegisterType<RuntimeSettings>().AsSelf().SingleInstance();
            this._containerBuilder.RegisterType<PluginProvider>().As<IPluginProvider>().SingleInstance();
            this._containerBuilder.RegisterGeneric(typeof(GlobalMessageChannel<>))
                                   .As(typeof(GlobalMessageChannel<>))
                                   .As(typeof(IMessageChannel<>))
                                   .SingleInstance();

            // Logger fallback
            this._containerBuilder.Register(_ => this._loggerFactory.CreateLogger<OrbitEngineBuilder>())
                                  .As<ILogger>()
                                  .SingleInstance();

            // Scheduling
            this._containerBuilder.RegisterType<EngineState>().AsSelf().SingleInstance();
            this._containerBuilder.RegisterType<ScheduleCalculator>().AsImplementedInterfaces().SingleInstance();
            this._containerBuilder.RegisterType<SimpleScheduler>().AsImplementedInterfaces().SingleInstance();
            this._containerBuilder.RegisterType<TextScheduleParser>().AsImplementedInterfaces().SingleInstance();

            // Core engine
            this._containerBuilder.RegisterType<OrbitEngine>().AsSelf().SingleInstance();

            // .NET DI interop
            this._containerBuilder.Register(c => new AutofacServiceProvider(c.Resolve<ILifetimeScope>()))
                                  .As<IServiceProvider>()
                                  .SingleInstance();

            // WeatherData access layer
            this._containerBuilder.RegisterType<ReflectiveInMemoryContext>().AsSelf().InstancePerLifetimeScope();
            this._containerBuilder.RegisterType<ReflectiveInMemoryDbAdapter>().As<IDbAdapter>().InstancePerLifetimeScope();

            // WeatherData access layer - localdb
            this._containerBuilder.RegisterType<LocalDbContext>().AsSelf().InstancePerLifetimeScope();
            this._containerBuilder.RegisterType<LocalDbAdapter>().As<IDbAdapter>().InstancePerLifetimeScope();

            this._containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            IContainer container = this._containerBuilder.Build();
            return container.Resolve<OrbitEngine>();
        }

        public OrbitEngineBuilder Configure(IConfiguration configuration)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            return this;
        }

        public OrbitEngineBuilder UseConfiguration(string settingsPath = "appsettings.json")
        {
            if (this._configuration is null)
            {
                if (!File.Exists(settingsPath))
                {
                    throw new FileNotFoundException($"Configuration file not found: {settingsPath}", settingsPath);
                }

                this._configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(settingsPath, optional: false, reloadOnChange: false)
                    .Build();

                this._rawConfiguration = this._configuration.GetSection("OrbitEngine").Get<RawEngineConfiguration>()!;
            }
            else { throw new InvalidOperationException("Configuration has already been set."); }

            return this;
        }

        public OrbitEngineBuilder UseConfiguration(RawEngineConfiguration rawConfiguration)
        {
            string json = JsonConvert.SerializeObject(rawConfiguration);

            if (this._configuration is null)
            {
                using MemoryStream stream = new(Encoding.UTF8.GetBytes(json));

                this._configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonStream(stream)
                    .Build();
            }
            else { throw new InvalidOperationException("Configuration has already been set."); }

            this._rawConfiguration = rawConfiguration;
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
