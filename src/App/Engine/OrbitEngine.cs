using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Logging;

namespace ORBIT9000.Engine
{
    /// <summary>
    /// Represents the core engine of the ORBIT9000 application, responsible for managing configuration and execution.
    /// </summary>
    public class OrbitEngine
    {
        private readonly IConfiguration _rawConfiguration;

        private ILogger<OrbitEngine> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrbitEngine"/> class with a preloaded configuration.
        /// </summary>
        /// <param name="configuration">The preloaded <see cref="OrbitEngineConfig"/> object.</param>
        /// <exception cref="NotImplementedException">Thrown because this constructor is not yet implemented.</exception>
        public OrbitEngine(OrbitEngineConfig configuration)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrbitEngine"/> class using a raw configuration.
        /// </summary>
        /// <param name="configuration">The raw configuration object implementing <see cref="IConfiguration"/>.</param>
        /// <exception cref="InvalidOperationException">Thrown if the provided configuration is null or empty.</exception>
        public OrbitEngine(IConfiguration configuration, ILogger<OrbitEngine> logger)
        {
            this._logger = logger;

            if (_logger is null)
                _logger = new DefaultConsoleLogger();

            _rawConfiguration = configuration;
        }

        /// <summary>
        /// The configuration object for the Orbit Engine.
        /// </summary>
        public OrbitEngineConfig? Configuration { get; private set; }
        /// <summary>
        /// Gets a value indicating whether the engine has been initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Executes the engine. If the engine is not initialized, it will initialize first.
        /// </summary>
        public void Start()
        {
            if (IsInitialized is false)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Initializes the engine. This method is called internally before execution if the engine is not already initialized.
        /// </summary>
        private void Initialize()
        {   

            if (_rawConfiguration is null || _rawConfiguration.AsEnumerable().Any() is false)
            {
                _logger.LogError("Configuration was NULL or EMPTY.");
                throw new InvalidOperationException("Configuration was NULL or EMPTY.");
            }

            RawOrbitEngineConfig? raw = _rawConfiguration.Get<RawOrbitEngineConfig>();

            if (raw is not null)
            {
                _logger.LogInformation("Loading raw configuration.");
                this.Configuration = OrbitEngineConfig.FromRaw(raw, _logger);
            }

            _logger.LogInformation("Checking whether any configuration exists.");

            if (File.Exists("orbit.config.json"))
            {
                _logger.LogInformation("Configuration file found.");
            }
            else
            {
                _logger.LogInformation("Configuration file not found. Creating...");
            }
        }
    }
}