using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;

namespace ORBIT9000.Engine.IO.Loaders.Plugin.Strategies
{
    internal class StringArrayPluginLoader(ILogger<StringArrayPluginLoader> logger, IAssemblyLoader loader)
        : PluginLoaderBase<string[]>(logger, loader)
    {
        public override IEnumerable<PluginInfo> LoadPlugins(string[] source)
        {
            foreach (string plugin in source)
            {
                yield return this.LoadSingle(plugin);
            }
        }
    }
}