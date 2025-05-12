using ORBIT9000.Engine.Configuration;

namespace ORBIT9000.Engine.IO.Loaders.Plugin
{
    public interface IPluginLoader<in TSource> : IPluginLoader
        where TSource : class
    {
        IEnumerable<PluginInfo> LoadPlugins(TSource source);
        IEnumerable<PluginInfo> LoadPlugins(DirectoryInfo source);
    }

    public interface IPluginLoader
    {
        IEnumerable<PluginInfo> LoadPlugins(object source);
        void Unload(object plugin);
    }
}
