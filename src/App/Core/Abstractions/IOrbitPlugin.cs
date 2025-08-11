using Microsoft.Extensions.DependencyInjection;

<<<<<<< HEAD
namespace ORBIT9000.Abstractions
=======
namespace ORBIT9000.Core.Abstractions
>>>>>>> bfa6c2d (Try fix pipeline)
{
    public interface IOrbitPlugin
    {
        Task OnLoad();
        Task OnUnload();
<<<<<<< HEAD
        Task<object> Execute();
=======

>>>>>>> bfa6c2d (Try fix pipeline)
        void RegisterServices(IServiceCollection collection) { }
    }
}
