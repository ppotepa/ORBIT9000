using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Configuration;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly;

<<<<<<<< HEAD:src/App/Engine/IO/Loaders/Plugin/Strategies/DirectoryPluginLoader.cs
namespace ORBIT9000.Engine.IO.Loaders.Plugin.Strategies
========
namespace ORBIT9000.Engine.Loaders.Plugin.Strategies
>>>>>>>> 7611f11 (Refactor plugin loading and configuration handling):src/App/Engine/Loaders/Plugin/Strategies/DirectoryPluginLoader.cs
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
                _logger.LogWarning("No plugins found in {Name}", source.FullName);
            }
            else
            {
                foreach (FileInfo file in source.GetFiles("*.dll", SearchOption.TopDirectoryOnly))
                {
                    yield return LoadSingle(file.FullName);
                }
            }
        }
    }
}