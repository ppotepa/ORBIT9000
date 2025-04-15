using Microsoft.Extensions.Configuration;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;

namespace ORBIT9000.Engine
{
    public class OrbitEngine
    {
        private readonly OrbitEngineConfig? _configuration;

        public OrbitEngine(OrbitEngineConfig configuration)
        {
            throw new NotImplementedException();
        }

        public OrbitEngine(IConfiguration configuration)
        {
            if (configuration is null || configuration.AsEnumerable().Any() is false)
                throw new InvalidOperationException("Configuration was NULL or EMPTY.");

            RawOrbitEngineConfig? raw = configuration.Get<RawOrbitEngineConfig>();

            if(raw is not null)
            {
                this._configuration = OrbitEngineConfig.FromRaw(raw);
            }
        }

        public bool IsInitialized { get; private set; }

        public void Execute()
        {
            if (IsInitialized is false)
            {
                Initialize();
            }
        }

        private void Initialize()
        {

        }
    }
}