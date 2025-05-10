using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;

namespace ORBIT9000.Engine.IO.Loaders.Plugin.Strategies
{
    internal class DirectoryPluginLoader(ILogger<DirectoryPluginLoader> logger, IAssemblyLoader loader)
        : PluginLoaderBase<DirectoryInfo>(logger, loader)
    {
        public override IEnumerable<PluginInfo> LoadPlugins(DirectoryInfo source)
        {
            if (!source.Exists)
                source.Create();

            FileInfo[] files = source.GetFiles("*.dll", SearchOption.TopDirectoryOnly);

            if (files.Length == 0)
            {
                this._logger.LogWarning("No plugins found in {Name}", source.FullName);
            }
            else
            {
                foreach (FileInfo file in source.GetFiles("*.dll", SearchOption.TopDirectoryOnly))
                {
                    yield return this.LoadSingle(file.FullName);
                }
            }
        }
    }
}