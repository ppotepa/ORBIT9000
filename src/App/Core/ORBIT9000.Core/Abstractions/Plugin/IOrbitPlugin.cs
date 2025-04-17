using Microsoft.Extensions.DependencyInjection;

namespace ORBIT9000.Core.Abstractions.Plugin
{
    public interface IOrbitPlugin
    {
        Type[] GetDataProviders();

        void Register(IServiceCollection services);
    }
}
