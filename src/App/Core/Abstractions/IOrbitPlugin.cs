using Microsoft.Extensions.DependencyInjection;

namespace ORBIT9000.Core.Abstractions
{
    public interface IOrbitPlugin
    {
        Task OnLoad();
        Task OnUnload();
        Task<object> Execute();

        void RegisterServices(IServiceCollection collection) { }
    }
}
