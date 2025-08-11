using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Loaders.Plugin.Results;

namespace ORBIT9000.Engine.Loaders.Plugin.Strategies
{
    internal class StringArrayPluginLoader : PluginLoaderBase<string[]>
    {
        public StringArrayPluginLoader(ILogger? logger, OrbitEngineConfiguration config) : base(logger, config)
        {
        }

        public override IEnumerable<PluginLoadResult> LoadPlugins(string[] source)
        {
            foreach (string plugin in source)
            {
                yield return LoadSingle(plugin);
            }
        }
    }
}