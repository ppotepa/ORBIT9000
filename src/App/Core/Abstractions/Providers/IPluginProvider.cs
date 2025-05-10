namespace ORBIT9000.Core.Abstractions.Providers
{
    public interface IPluginProvider
    {
        IEnumerable<Type> Plugins { get; }

        Task<IOrbitPlugin> Activate(Type plugin, bool executeOnLoad = false);
        Task<IOrbitPlugin> Activate(object plugin, bool executeOnLoad = false);

        void Unload(object plugin);
    }
}