using Microsoft.Extensions.DependencyInjection;

namespace ORBIT9000.Core.Abstractions.Loaders
{
    public interface IOrbitPlugin
    {        
        Task OnLoad();
        Task OnUnload();

        void RegisterServices(IServiceCollection collection) { }
    }
}
