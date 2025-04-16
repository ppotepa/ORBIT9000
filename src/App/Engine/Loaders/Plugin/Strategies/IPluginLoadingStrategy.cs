using ORBIT9000.Engine.Loaders.Plugin.Results;

namespace ORBIT9000.Engine.Loaders.Plugin.Strategies.ORBIT9000.Engine.Loaders.Plugin.Strategies
{
    internal interface IPluginLoadingStrategy<in TSource>
    {
        IEnumerable<PluginLoadResult> LoadPlugins(TSource source);
    }
}