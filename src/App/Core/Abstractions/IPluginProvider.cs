using ORBIT9000.Core.Abstractions;

namespace ORBIT9000.Abstractions
{
    public interface IPluginProvider
    {
        Task<IOrbitPlugin> Activate(Type plugin);
        Task<IOrbitPlugin> Activate(object plugin);
        void Unload(object plugin);
    }
}