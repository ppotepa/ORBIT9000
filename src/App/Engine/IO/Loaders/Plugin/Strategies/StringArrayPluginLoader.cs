using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Loaders;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Loaders.Plugin.Results;

namespace ORBIT9000.Engine.IO.Loaders.Plugin.Strategies
{
    internal class StringArrayPluginLoader : PluginLoaderBase<string[]>
    {
        public StringArrayPluginLoader(ILogger? logger, RawConfiguration config, IAssemblyLoader loader) : base(logger, config, loader)
        {
        }

        public override IEnumerable<AssemblyLoadResult> LoadPlugins(string[] source)
        {
            foreach (string plugin in source)
            {
                yield return LoadSingle(plugin);
            }
        }
    }
}