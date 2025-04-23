using ORBIT9000.Core.Abstractions.Loaders;

namespace ORBIT9000.Abstractions
{
    public interface IPluginProvider
    {
        IOrbitPlugin Activate(Type plugin); 
        IOrbitPlugin Activate(object plugin); 
    }
}