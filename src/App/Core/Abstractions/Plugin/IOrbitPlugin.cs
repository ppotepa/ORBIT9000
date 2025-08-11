using Microsoft.Extensions.DependencyInjection;

namespace ORBIT9000.Core.Abstractions
{
    public interface IOrbitPlugin
    {
        Task OnLoad();
        Task OnUnload();

        void RegisterServices(IServiceCollection collection) { }
    }
}
