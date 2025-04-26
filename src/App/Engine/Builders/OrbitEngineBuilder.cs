using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ORBIT9000.Abstractions;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.IO.Loaders.Plugin;
using ORBIT9000.Engine.IO.Loaders.Plugin.Strategies;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;
using ORBIT9000.Engine.Providers;
using System.Text;

namespace ORBIT9000.Engine.Builders
{
    public class OrbitEngineBuilder
    {
        private readonly ContainerBuilder _containerBuilder;
        private readonly ILogger<OrbitEngineBuilder>? _logger;
        private readonly ILoggerFactory _loggerFactory;

        private IConfiguration? _configuration;
        private RawEngineConfiguration? _rawConfiguration;

        public OrbitEngineBuilder(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<OrbitEngineBuilder>();
            _logger.LogDebug("OrbitEngineBuilder initialized");

            _containerBuilder = new ContainerBuilder();
        }
      
        public OrbitEngine Build()
        {
            _containerBuilder.RegisterInstance(_loggerFactory).As<ILoggerFactory>();

            _ = _containerBuilder.RegisterGeneric((context, genericArguments, parameters) =>
            {
                _logger!.LogInformation("Trying to factorize Logger for {Type}", genericArguments[0].Name);

                var loggerFactory = context.Resolve<ILoggerFactory>();

                var method = typeof(LoggerFactoryExtensions)
                    .GetMethods()
                    .First(m => m.Name == "CreateLogger" && m.IsGenericMethod && m.GetParameters().Length == 1);

                var genericMethod = method.MakeGenericMethod(genericArguments[0]);

                return genericMethod.Invoke(null, [loggerFactory])!;
            })
            .As(typeof(ILogger<>))
            .InstancePerDependency();

            _containerBuilder.RegisterInstance(_configuration!).As<IConfiguration>();
            _containerBuilder.RegisterInstance(_rawConfiguration!).AsSelf();
            _containerBuilder.RegisterInstance(_containerBuilder).AsSelf();

            _containerBuilder.RegisterType<StringArrayPluginLoader>().AsSelf();
            _containerBuilder.RegisterType<DebugDirectoryPluginLoader>().AsSelf();
            _containerBuilder.RegisterType<DirectoryPluginLoader>().AsSelf();
            _containerBuilder.RegisterType<PluginLoaderFactory>().AsSelf();

            _containerBuilder.RegisterType<AssemblyLoader>().As<IAssemblyLoader>().SingleInstance();

            _containerBuilder.RegisterType<RuntimeConfiguration>().AsSelf().SingleInstance();
            _containerBuilder.RegisterType<PluginProvider>().As<IPluginProvider>().SingleInstance();

            _containerBuilder.Register(c => _loggerFactory.CreateLogger<OrbitEngineBuilder>()).As<ILogger>().SingleInstance();

            _containerBuilder.Register(c =>
            {
                PluginLoaderFactory factory = c.Resolve<PluginLoaderFactory>();
                return factory.Create();
            })
            .As<IPluginLoader>()
            .SingleInstance();

            _containerBuilder.RegisterType<OrbitEngine>()
                .AsSelf()
                .SingleInstance();

            _containerBuilder.Register(c => new AutofacServiceProvider(c.Resolve<ILifetimeScope>()))
                .As<IServiceProvider>()
                .SingleInstance();

            return _containerBuilder.Build().BeginLifetimeScope()
                .Resolve<OrbitEngine>();
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
    }
}
