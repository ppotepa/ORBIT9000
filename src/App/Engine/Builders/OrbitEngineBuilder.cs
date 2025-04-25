using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Environment;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.IO.Loaders;
using ORBIT9000.Engine.IO.Loaders.Plugin;
using ORBIT9000.Engine.IO.Loaders.Plugin.Strategies;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;
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
        private RuntimeConfiguration? _internalOrbitEngineConfig;
        private ILoggerFactory _loggerFactory;
        private RawEngineConfiguration? _rawConfiguration;
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
            _services.AddSingleton(_services);

            _services.AddTransient<StringArrayPluginLoader>();
            _services.AddTransient<DebugDirectoryPluginLoader>();
            _services.AddTransient<DirectoryPluginLoader>();
            _services.AddTransient<PluginLoaderFactory>();

            _services.AddSingleton<IAssemblyLoader, AssemblyLoader>();

            _services.AddSingleton<RuntimeConfiguration>();
            _services.AddSingleton<IPluginProvider, PluginProvider>();
            _services.AddLogging();

            _services.AddSingleton<IPluginLoader>(provider
                =>
            {
                return provider.GetRequiredService<PluginLoaderFactory>().Create();
            });


            _services.AddSingleton<OrbitEngine>();

            return _services.BuildServiceProvider().GetRequiredService<OrbitEngine>();
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

                _rawConfiguration = _configuration.GetSection("OrbitEngine").Get<RawEngineConfiguration>();
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
