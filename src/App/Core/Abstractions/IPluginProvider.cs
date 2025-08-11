using ORBIT9000.Core.Abstractions;
using ORBIT9000.Core.Events;
using System.Threading.Channels;

namespace ORBIT9000.Abstractions
{
    public interface IPluginProvider
    {
        ChannelReader<PluginEvent> PluginEvents { get; }
        Task<IOrbitPlugin> Activate(Type plugin);
        Task<IOrbitPlugin> Activate(object plugin);
        void Unload(object plugin);
    }
}