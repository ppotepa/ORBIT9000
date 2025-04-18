using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ORBIT9000.Engine.Extensions
{
    public static class DependencyInjection
    {
        public static void AddOrbitEngine(this IServiceCollection services
            , IConfiguration config, ILogger logger)
        {
            OrbitEngine orbitEngine = new OrbitEngine();
            orbitEngine.Initialize();
            services.AddSingleton(orbitEngine);
        }   
    }
}
