using ORBIT9000.Core.Abstractions.Loaders;

namespace ORBIT9000.Abstractions
{
    public interface IPluginProvider
    {
        IOrbitPlugin Activate(Type plugin);
        Type[] GetPluginRegistrationInfo();
        IOrbitPlugin Register(Type plugin);        
    }
}