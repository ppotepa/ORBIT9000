using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework.Internal;
using ORBIT9000.Core.Abstractions.Providers;
using ORBIT9000.Core.Abstractions.Scheduling;
using ORBIT9000.Engine.Builders;
using ORBIT9000.Engine.Configuration.Raw;
using System.Reflection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ORBIT9000.Engine.Tests
{
    [TestFixture]
    public class OrbitEngineBuilderTests
    {
        #region Fields

        private Mock<ILoggerFactory> _mockLoggerFactory;

        #endregion Fields

        #region Methods

        [Test]
        public void Build_RegistersAllExpectedDependencies()
        {
            CreateTestConfigFile();
            OrbitEngineBuilder builder = new(this._mockLoggerFactory.Object);
            builder.UseConfiguration("test_appsettings.json");

            OrbitEngine engine = builder.Build();

            Assert.That(engine, Is.Not.Null);
            Assert.That(engine, Is.InstanceOf<OrbitEngine>());

            File.Delete("test_appsettings.json");
        }

        [Test]
        public void Build_RegistersLoggerFactory()
        {
            CreateTestConfigFile();
            OrbitEngineBuilder builder = new(this._mockLoggerFactory.Object);
            builder.UseConfiguration("test_appsettings.json");

            OrbitEngine engine = builder.Build();

            Assert.That(engine, Is.Not.Null);

            FieldInfo? field = typeof(OrbitEngine).GetField("_logger", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(field, Is.Not.Null, "Field '_logger' not found.");
            object? logger = field?.GetValue(engine);
            Assert.That(logger, Is.InstanceOf<ILogger>());

            File.Delete("test_appsettings.json");
        }

        [Test]
        public void Build_RegistersPluginProviderAndScheduler()
        {
            CreateTestConfigFile();
            OrbitEngineBuilder builder = new(this._mockLoggerFactory.Object);
            builder.UseConfiguration("test_appsettings.json");

            OrbitEngine engine = builder.Build();

            Assert.Multiple(() =>
            {
                Assert.That(engine, Is.Not.Null);
                Assert.That(engine.PluginProvider, Is.InstanceOf<IPluginProvider>());

                Assert.That(engine.Scheduler, Is.Not.Null, "Scheduler property is null.");
                Assert.That(engine.Scheduler, Is.InstanceOf<IScheduler>());
            });

            File.Delete("test_appsettings.json");
        }

        [Test]
        public void Build_RegistersSingletonDependencies()
        {
            CreateTestConfigFile();
            OrbitEngineBuilder builder = new(this._mockLoggerFactory.Object);
            builder.UseConfiguration("test_appsettings.json");

            OrbitEngine engine = builder.Build();

            Assert.That(engine, Is.Not.Null);

            IServiceProvider serviceProvider = engine.ServiceProvider;

            object? pluginProvider1 = serviceProvider.GetService(typeof(IPluginProvider));
            object? pluginProvider2 = serviceProvider.GetService(typeof(IPluginProvider));

            Assert.Multiple(() =>
            {
                Assert.That(pluginProvider1, Is.InstanceOf<IPluginProvider>());
                Assert.That(pluginProvider2, Is.InstanceOf<IPluginProvider>());
                Assert.That(pluginProvider1, Is.SameAs(pluginProvider2));
            });

            File.Delete("test_appsettings.json");
        }

        [Test]
        public void Build_WithoutConfiguration_ThrowsArgumentNullException()
        {
            OrbitEngineBuilder builder = new(this._mockLoggerFactory.Object);
            Assert.Throws<ArgumentNullException>(() => builder.Build());
        }

        [Test]
        public void Configure_WithNullConfiguration_ThrowsArgumentNullException()
        {
            OrbitEngineBuilder builder = new(this._mockLoggerFactory.Object);
            Assert.Throws<ArgumentNullException>(() => builder.Configure(null!));
        }

        [Test]
        public void Configure_WithValidConfiguration_ReturnsBuilder()
        {
            OrbitEngineBuilder builder = new(this._mockLoggerFactory.Object);
            IConfiguration mockConfiguration = new Mock<IConfiguration>().Object;

            OrbitEngineBuilder result = builder.Configure(mockConfiguration);

            Assert.That(result, Is.EqualTo(builder));
        }

        [Test]
        public void Constructor_WithNullLoggerFactory_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new OrbitEngineBuilder(null!));
        }

        [Test]
        public void CreateLoggerFactory_GeneratesFactory_ForDifferentTypes()
        {
            CreateTestConfigFile();
            OrbitEngineBuilder builder = new(this._mockLoggerFactory.Object);
            builder.UseConfiguration("test_appsettings.json");

            OrbitEngine engine = builder.Build();

            Assert.That(engine, Is.Not.Null);

            IServiceProvider serviceProvider = engine.ServiceProvider;
            object? pluginProvider = serviceProvider.GetService(typeof(IPluginProvider));
            object? scheduler = serviceProvider.GetService(typeof(IScheduler));

            Assert.Multiple(() =>
            {
                Assert.That(pluginProvider, Is.InstanceOf<IPluginProvider>());
                Assert.That(scheduler, Is.InstanceOf<IScheduler>());
            });

            File.Delete("test_appsettings.json");
        }

        [SetUp]
        public void Setup()
        {
            this._mockLoggerFactory = new Mock<ILoggerFactory>();

            this._mockLoggerFactory
                .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns((string _) =>
                {
                    Mock<ILogger> mockLogger = new();
                    mockLogger
                        .Setup(logger => logger.Log(
                            It.IsAny<LogLevel>(),
                            It.IsAny<EventId>(),
                            It.IsAny<It.IsAnyType>(),
                            It.IsAny<Exception?>(),
                            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()))
                        .Verifiable();
                    return mockLogger.Object;
                });
        }

        [Test]
        public void UseConfiguration_WhenConfigurationAlreadySet_ThrowsInvalidOperationException()
        {
            OrbitEngineBuilder builder = new(this._mockLoggerFactory.Object);
            IConfiguration mockConfiguration = new Mock<IConfiguration>().Object;
            builder.Configure(mockConfiguration);

            Assert.Throws<InvalidOperationException>(() =>
                builder.UseConfiguration("appsettings.json"));
        }

        [Test]
        public void UseConfiguration_WithNonExistentFile_ThrowsFileNotFoundException()
        {
            OrbitEngineBuilder builder = new(this._mockLoggerFactory.Object);
            const string nonExistentFile = "nonexistent_settings.json";

            Assert.Throws<FileNotFoundException>(() =>
                builder.UseConfiguration(nonExistentFile));
        }

        [Test]
        public void UseConfiguration_WithRawConfiguration_CreatesConfiguration()
        {
            OrbitEngineBuilder builder = new(this._mockLoggerFactory.Object);
            RawEngineConfiguration rawConfig = new()
            {
                EnableTerminal = true,
                SharePluginScopes = false,
                Plugins = new PluginsConfiguration()
                {
                    AbortOnError = true,
                    ActivePlugins = ["Plugin1", "Plugin2"],
                    LoadAsBinary = true
                }
            };

            OrbitEngineBuilder result = builder.UseConfiguration(rawConfig);

            Assert.That(result, Is.EqualTo(builder));
        }

        [Test]
        public void UseConfiguration_WithRawConfiguration_ThrowsWhenConfigurationAlreadySet()
        {
            OrbitEngineBuilder builder = new(this._mockLoggerFactory.Object);
            IConfiguration mockConfiguration = new Mock<IConfiguration>().Object;
            builder.Configure(mockConfiguration);

            RawEngineConfiguration rawConfig = new()
            {
                EnableTerminal = true,
                SharePluginScopes = false,
                Plugins = new PluginsConfiguration()
                {
                    AbortOnError = true,
                    ActivePlugins = ["Plugin1", "Plugin2"],
                    LoadAsBinary = true
                }
            };

            Assert.Throws<InvalidOperationException>(() =>
                builder.UseConfiguration(rawConfig));
        }

        private static void CreateTestConfigFile()
        {
            var config = new
            {
                OrbitEngine = new RawEngineConfiguration()
                {
                    EnableTerminal = true,
                    SharePluginScopes = false,
                    Plugins = new PluginsConfiguration()
                    {
                        AbortOnError = true,
                        ActivePlugins = ["./Binaries/ExamplePlugin.dll"],
                        LoadAsBinary = true
                    }
                }
            };

            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText("test_appsettings.json", json);
        }

        #endregion Methods
    }
}
