using Microsoft.Extensions.DependencyInjection;

namespace ORBIT9000.Abstractions
{
    public interface IOrbitPlugin
    {
        Task OnLoad();
        Task OnUnload();
        Task<object> Execute();
        void RegisterServices(IServiceCollection collection) { }
    }
}
