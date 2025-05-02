using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Runtime.State;

namespace ORBIT9000.Engine
{
    public partial class OrbitEngine
    {
        private readonly ILogger<OrbitEngine> _logger;
        private readonly Thread _mainThread;
        private readonly IPluginProvider _pluginProvider;
        private readonly IServiceProvider _serviceProvider;

        public OrbitEngine(
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider,
            RuntimeSettings configuration,
            IPluginProvider pluginProvider
            )
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(loggerFactory);
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(pluginProvider);

            _logger = loggerFactory.CreateLogger<OrbitEngine>()
                ?? throw new InvalidOperationException("Logger could not be created.");

            _mainThread = new Thread(Strategies.Running.Default.EngineStartupStrategy);
            _mainThread.IsBackground = true;

            _mainThread.Name = "MainEngineThread";

            _pluginProvider = pluginProvider;
            _serviceProvider = serviceProvider;

            IsInitialized = true;
            IsRunning = true;
            _configuration = configuration;
            _logger.LogInformation("Engine initialized with configuration: {Configuration}", configuration);

        }

        public bool IsInitialized { get; }

        private RuntimeSettings _configuration;

        public bool IsRunning { get; private set; }
        public IPluginProvider PluginProvider { get => _pluginProvider; }
        public IServiceProvider ServiceProvider { get => _serviceProvider; }
        internal RuntimeSettings Configuration { get => _configuration; set => _configuration = value; }
        public void Start()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Engine has not been initialized.");

            _logger.LogInformation("Starting engine thread...");
            _mainThread.Start(_serviceProvider.GetAutofacRoot().Resolve<EngineState>());

            while(IsRunning)
            {
                Thread.Sleep(100);
            }   
        }
    }
}