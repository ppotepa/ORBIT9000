using ORBIT9000.Engine.Loaders.Plugin.Results;

namespace ORBIT9000.Engine.IO.Loaders.Plugin
{
    public interface IPluginLoader<in TSource> : IPluginLoader
        where TSource : class
    {
        IEnumerable<AssemblyLoadResult> LoadPlugins(TSource source);
    }

    public interface IPluginLoader
    {
        IEnumerable<AssemblyLoadResult> LoadPlugins(object source);
    }
}
