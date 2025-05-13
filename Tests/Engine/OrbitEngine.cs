using Autofac;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;
using ORBIT9000.Abstractions.Providers;
using ORBIT9000.Abstractions.Scheduling;
using ORBIT9000.Core.Environment;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Tests.TestHelpers.Logging;

namespace ORBIT9000.Engine.Tests
{
    [TestFixture]
    public class OrbitEngine : Disposable
    {
        #region Fields

        private Mock<RuntimeSettings> _configuration;
        private ILoggerFactory _inMemoryLoggerFactory;
        private Mock<IPluginProvider> _mockPluginProvider;
        private Mock<IScheduler> _mockScheduler;
        private Mock<IServiceProvider> _mockServiceProvider;

        #endregion Fields

        #region Methods

        [Test]
        public void Constructor_InitializesEngine_WithValidDependencies()
        {
            Engine.OrbitEngine engine = new(
                 this._inMemoryLoggerFactory,
                 this._mockServiceProvider.Object,
                 this._configuration.Object,
                 this._mockPluginProvider.Object,
                 this._mockScheduler.Object);

            Assert.Multiple(() =>
            {
                Assert.That(engine.IsInitialized, Is.True);
                Assert.That(engine.IsRunning, Is.True);
                Assert.That(engine.PluginProvider, Is.EqualTo(this._mockPluginProvider.Object));
                Assert.That(engine.ServiceProvider, Is.EqualTo(this._mockServiceProvider.Object));
                Assert.That(engine.Scheduler, Is.EqualTo(this._mockScheduler.Object));
            });
        }

        [Test]
        public void Constructor_ThrowsArgumentNullException_WhenDependenciesAreNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Engine.OrbitEngine(null!, this._mockServiceProvider.Object, this._configuration.Object, this._mockPluginProvider.Object, this._mockScheduler.Object));
            Assert.Throws<ArgumentNullException>(() => new Engine.OrbitEngine(this._inMemoryLoggerFactory, null!, this._configuration.Object, this._mockPluginProvider.Object, this._mockScheduler.Object));
            Assert.Throws<ArgumentNullException>(() => new Engine.OrbitEngine(this._inMemoryLoggerFactory, this._mockServiceProvider.Object, null!, this._mockPluginProvider.Object, this._mockScheduler.Object));
            Assert.Throws<ArgumentNullException>(() => new Engine.OrbitEngine(this._inMemoryLoggerFactory, this._mockServiceProvider.Object, this._configuration.Object, null!, this._mockScheduler.Object));
            Assert.Throws<ArgumentNullException>(() => new Engine.OrbitEngine(this._inMemoryLoggerFactory, this._mockServiceProvider.Object, this._configuration.Object, this._mockPluginProvider.Object, null!));
        }

        [Test]
        public void LoggingMethods_CallUnderlyingLogger()
        {
            InMemoryLoggerProvider inMemoryLoggerProvider = new();
            ILoggerFactory loggerFactory = inMemoryLoggerProvider.CreateLoggerFactory();

            Engine.OrbitEngine engine = new(
                loggerFactory,
                this._mockServiceProvider.Object,
                this._configuration.Object,
                this._mockPluginProvider.Object,
                this._mockScheduler.Object);

            const string testMessage = "Test message {A}";

            object[] testArgs = ["arg1"];

            const string expectedMessage = "Test message {A} arg1";

            engine.LogTrace(testMessage, testArgs);
            engine.LogDebug(testMessage, testArgs);
            engine.LogInformation(testMessage, testArgs);
            engine.LogWarning(testMessage, testArgs);
            engine.LogError(testMessage, testArgs);
            engine.LogCritical(testMessage, testArgs);

            List<LogEntry> logs = inMemoryLoggerProvider.Entries;

            // We are expecting 7 because on of the log entries come from the Engine itself.
            Assert.Multiple(() =>
            {
                Assert.That(logs, Has.Count.EqualTo(7));
                Assert.That(logs.Any(l => l.Level == LogLevel.Trace && l.Message == expectedMessage), Is.True, "Trace log wasn't found");
                Assert.That(logs.Any(l => l.Level == LogLevel.Debug && l.Message == expectedMessage), Is.True, "Debug log wasn't found");
                Assert.That(logs.Any(l => l.Level == LogLevel.Information && l.Message == expectedMessage), Is.True, "Information log wasn't found");
                Assert.That(logs.Any(l => l.Level == LogLevel.Warning && l.Message == expectedMessage), Is.True, "Warning log wasn't found");
                Assert.That(logs.Any(l => l.Level == LogLevel.Error && l.Message == expectedMessage), Is.True, "Error log wasn't found");
                Assert.That(logs.Any(l => l.Level == LogLevel.Critical && l.Message == expectedMessage), Is.True, "Critical log wasn't found");
                Assert.That(logs.Any(l => l.Category!.Equals("ORBIT9000.Engine.OrbitEngine")), Is.True, "ORBIT9000.Engine.OrbitEngine log wasn't found");
            });
        }

        [SetUp]
        public void Setup()
        {
            this._inMemoryLoggerFactory = new InMemoryLoggerProvider().CreateLoggerFactory();
            this._mockServiceProvider = new Mock<IServiceProvider>();
            this._mockPluginProvider = new Mock<IPluginProvider>();
            this._mockScheduler = new Mock<IScheduler>();
            this._configuration = new Mock<RuntimeSettings>();

            this._mockServiceProvider.Setup(provider => provider.GetService(typeof(ILifetimeScope)))
                .Returns(new Mock<ILifetimeScope>().Object);
        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing)
            {
                this._inMemoryLoggerFactory?.Dispose();
                this._mockServiceProvider = null!;
                this._mockPluginProvider = null!;
                this._mockScheduler = null!;
                this._configuration = null!;
            }

            base.Dispose(disposing);
        }

        #endregion Methods
    }
}