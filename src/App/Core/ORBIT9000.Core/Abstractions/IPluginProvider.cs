using ORBIT9000.Core.Abstractions;

namespace ORBIT9000.Abstractions
{
    public interface IPluginProvider
    {
        IOrbitPlugin Activate(Type plugin);

        IOrbitPlugin Activate(object plugin);
        void Unload(object plugin);
    }
}