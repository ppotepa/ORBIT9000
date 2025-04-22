using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ORBIT9000.Core.Abstractions.Loaders;
using ORBIT9000.Core.Environment;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Loaders;
using ORBIT9000.Engine.Loaders.Plugin;
using ORBIT9000.Engine.Loaders.Plugin.Strategies;
using ORBIT9000.Engine.Loaders.PluginAssembly;
using ORBIT9000.Engine.Providers;
using System.Text;

namespace ORBIT9000.Engine.Builders
{
    public class OrbitEngineBuilder
    {
        private readonly ILogger<OrbitEngineBuilder>? _logger;
        private readonly Dictionary<Type, PluginRegistrationInfo> _plugins = new();
        private readonly IServiceCollection _services = new ServiceCollection();
        private IConfiguration? _configuration;
        private InitializedInternalConfig? _internalOrbitEngineConfig;
        private ILoggerFactory _loggerFactory;
        private RawConfiguration? _rawConfiguration;
        public OrbitEngineBuilder(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = _loggerFactory.CreateLogger<OrbitEngineBuilder>();
            _logger.LogDebug("OrbitEngineBuilder initialized");
        }

        public OrbitEngine Build()
        {
            _services.AddSingleton(_loggerFactory);
            _services.AddSingleton(_configuration);
            _services.AddSingleton(_rawConfiguration);

            _services.AddTransient<StringArrayPluginLoader>();
            _services.AddTransient<DebugDirectoryPluginLoader>();
            _services.AddTransient<DirectoryPluginLoader>();
            _services.AddSingleton<IAssemblyLoader, AssemblyLoader>();

            _services.AddSingleton<InitializedInternalConfig>();
            _services.AddSingleton<IPluginProvider, PluginProvider>();
            _services.AddLogging();

            _ = _services.AddSingleton(typeof(IPluginLoader), static provider =>
            {
                RawConfiguration config = provider.GetService<RawConfiguration>();

                return config.OrbitEngine.Plugins.ActivePlugins.Length switch
                {
                    > 0 => provider.GetService<StringArrayPluginLoader>(),
                    _ when AppEnvironment.IsDebug => provider.GetService<DebugDirectoryPluginLoader>(),
                    _ => provider.GetService<DirectoryPluginLoader>()
                };
            });

            _services.AddSingleton<OrbitEngine>();

            return _services.BuildServiceProvider().GetService<OrbitEngine>();
        }

        public OrbitEngineBuilder Configure(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            return this;
        }

        public OrbitEngineBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            configureServices?.Invoke(_services);
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

                _rawConfiguration = _configuration.Get<RawConfiguration>();
            }
            else { throw new InvalidOperationException("Configuration has already been set."); }

            return this;
        }

        public OrbitEngineBuilder UseConfiguration(RawConfiguration rawConfiguration)
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
