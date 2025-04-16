using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Loaders.Plugin.Results;
using ORBIT9000.Engine.Loaders.Plugin.Strategies.ORBIT9000.Engine.Loaders.Plugin.Strategies;

namespace ORBIT9000.Engine.Loaders.Plugin.Strategies
{
    internal class LoadFromStringArrayStrategy : PluginLoadingStrategy<string[]>
    {
        public LoadFromStringArrayStrategy(Configuration.Raw.RawOrbitEngineConfig rawConfig, ILogger? logger) : base(logger)
        {
        }

        public override IEnumerable<PluginLoadResult> LoadPlugins(string[] source)
        {
            foreach(var plugin in source)
            {
                yield return LoadSingle(plugin, true);
            }
        }
    }
}