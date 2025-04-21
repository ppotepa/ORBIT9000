using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Environment;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Loaders.Plugin.Results;
using ORBIT9000.Engine.Loaders.Plugin.Strategies;

namespace ORBIT9000.Engine.Loaders.Plugin
{
    internal static class PluginLoaderFactory
    {
        public static IEnumerable<PluginLoadResult> Create(OrbitEngineConfiguration config, ILogger? logger = default)
        {
            return config.OrbitEngine.Plugins.ActivePlugins.Length switch
            {
                > 0 => new StringArrayPluginLoader(logger, config).LoadPlugins(config.OrbitEngine.Plugins.ActivePlugins),
                _ when AppEnvironment.IsDebug => new DebugDirectoryPluginLoader(logger, config).LoadPlugins(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)),
                _ => new DirectoryPluginLoader(logger, config).LoadPlugins(new DirectoryInfo("./Plugins"))
            };
        }
    }
}
