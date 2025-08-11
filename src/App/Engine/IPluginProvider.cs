using ORBIT9000.Core.Abstractions.Loaders;
using ORBIT9000.Engine.Loaders.Plugin.Results;

namespace ORBIT9000.Engine
{
    public interface IPluginProvider
    {
        IOrbitPlugin Activate(Type plugin);
        IOrbitPlugin Register(Type plugin);
        AssemblyLoadResult[] GetPluginRegistrationInfo();
    }
}