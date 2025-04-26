using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;

namespace ORBIT9000.Engine.IO.Loaders.Plugin.Strategies
{
    internal class StringArrayPluginLoader : PluginLoaderBase<string[]>
    {
        public StringArrayPluginLoader(ILogger<StringArrayPluginLoader> logger,  IAssemblyLoader loader) : base(logger, loader)
        {
        }

        public override IEnumerable<PluginInfo> LoadPlugins(string[] source)
        {
            foreach (string plugin in source)
            {
                yield return LoadSingle(plugin);
            }
        }
    }
}