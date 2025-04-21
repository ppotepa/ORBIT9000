using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Loaders.Plugin.Results;

namespace ORBIT9000.Engine.Loaders.Plugin.Strategies
{
    internal class DirectoryPluginLoader : PluginLoaderBase<DirectoryInfo>
    {
        public DirectoryPluginLoader(ILogger? logger) : base(logger)
        {
        }

        public override IEnumerable<PluginLoadResult> LoadPlugins(DirectoryInfo source)
        {
            if (!source.Exists)
                source.Create();

            FileInfo[] files = source.GetFiles("*.dll", SearchOption.TopDirectoryOnly);

            if (files.Length == 0)
            {
                this._logger?.LogWarning($"No plugins found in {source.FullName}");
                yield break;
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