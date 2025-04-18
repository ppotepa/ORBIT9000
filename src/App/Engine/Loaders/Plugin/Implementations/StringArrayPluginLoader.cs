using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Loaders.Plugin.Results;

namespace ORBIT9000.Engine.Loaders.Plugin.Implementations
{
    internal class StringArrayPluginLoader : PluginLoaderBase<string[]>
    {
        public StringArrayPluginLoader(ILogger? logger) : base(logger)
        {
        }

        public override IEnumerable<PluginLoadResult> LoadPlugins(string[] source)
        {
            foreach (var plugin in source)
            {
                yield return LoadSingle(plugin);
            }
        }
    }
}