using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Loaders.Plugin.Results;
using ORBIT9000.Engine.Loaders.Plugin.Strategies.ORBIT9000.Engine.Loaders.Plugin.Strategies;

namespace ORBIT9000.Engine.Loaders.Plugin.Strategies
{
    internal class LoadFromDirectoryStrategy : PluginLoadingStrategy<DirectoryInfo>
    {
        public LoadFromDirectoryStrategy(Configuration.Raw.RawOrbitEngineConfig rawConfig, ILogger? logger) : base(logger)
        {
        }

        public override IEnumerable<PluginLoadResult> LoadPlugins(DirectoryInfo source)
        {
            foreach(var file in source.GetFiles("*.dll", SearchOption.TopDirectoryOnly))
            {
                yield return LoadSingle(file.FullName, true);
            }
        }
    }

}