namespace ORBIT9000.Core.Abstractions.Providers
{
    public interface IPluginProvider
    {
        IEnumerable<Type> Plugins { get; }

        Task<IOrbitPlugin> Activate(Type plugin);
        Task<IOrbitPlugin> Activate(object plugin);

        void Unload(object plugin);
    }
}