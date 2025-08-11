using Microsoft.Extensions.Logging;
<<<<<<< HEAD
<<<<<<< HEAD
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;

namespace ORBIT9000.Engine.IO.Loaders.Plugin.Strategies
{
    internal class StringArrayPluginLoader(ILogger<StringArrayPluginLoader> logger, IAssemblyLoader loader)
        : PluginLoaderBase<string[]>(logger, loader)
    {
        public override IEnumerable<PluginInfo> LoadPlugins(string[] source)
=======
using ORBIT9000.Core.Abstractions.Loaders;
=======
using ORBIT9000.Engine.Configuration;
>>>>>>> 254394d (Remove OverLogging)
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;

namespace ORBIT9000.Engine.IO.Loaders.Plugin.Strategies
{
    internal class StringArrayPluginLoader : PluginLoaderBase<string[]>
    {
        public StringArrayPluginLoader(ILogger<StringArrayPluginLoader> logger, RawConfiguration config, IAssemblyLoader loader) : base(logger, config, loader)
        {
        }

<<<<<<< HEAD
        public override IEnumerable<AssemblyLoadResult> LoadPlugins(string[] source)
>>>>>>> e2b2b5a (Reworked Naming)
=======
        public override IEnumerable<PluginInfo> LoadPlugins(string[] source)
>>>>>>> 254394d (Remove OverLogging)
        {
            foreach (string plugin in source)
            {
                yield return LoadSingle(plugin);
            }
        }
    }
}