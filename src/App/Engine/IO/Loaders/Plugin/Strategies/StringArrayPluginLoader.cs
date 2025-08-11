using Microsoft.Extensions.Logging;
<<<<<<< HEAD
<<<<<<< HEAD
using ORBIT9000.Engine.Configuration;
<<<<<<< HEAD
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
=======
>>>>>>> 53879fa (Add AutoInitialization to PluginProvider)
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;

namespace ORBIT9000.Engine.IO.Loaders.Plugin.Strategies
{
    internal class StringArrayPluginLoader(ILogger<StringArrayPluginLoader> logger, IAssemblyLoader loader)
        : PluginLoaderBase<string[]>(logger, loader)
    {
<<<<<<< HEAD
        public StringArrayPluginLoader(ILogger<StringArrayPluginLoader> logger, IAssemblyLoader loader) : base(logger, loader)
        {
        }

<<<<<<< HEAD
        public override IEnumerable<AssemblyLoadResult> LoadPlugins(string[] source)
>>>>>>> e2b2b5a (Reworked Naming)
=======
=======
>>>>>>> bfa6c2d (Try fix pipeline)
        public override IEnumerable<PluginInfo> LoadPlugins(string[] source)
>>>>>>> 254394d (Remove OverLogging)
        {
            foreach (string plugin in source)
            {
                yield return this.LoadSingle(plugin);
            }
        }
    }
}