using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.Loaders.Plugin.Results;

namespace ORBIT9000.Engine.IO.Loaders.Plugin
{
    internal static class PluginLoaderFactory
    {
        public static IEnumerable<AssemblyLoadResult> Create(Configuration.Raw.RawConfiguration config, ILogger? logger = default)
        {
            //return config.OrbitEngine.Plugins.ActivePlugins.Length switch
            //{
            //    > 0 => new StringArrayPluginLoader(logger, config).LoadPlugins(config.OrbitEngine.Plugins.ActivePlugins),
            //    _ when AppEnvironment.IsDebug => new DebugDirectoryPluginLoader(logger, config).LoadPlugins(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)),
            //    _ => new DirectoryPluginLoader(logger, config).LoadPlugins(new DirectoryInfo("./Plugins"))
            //};

            throw new NotImplementedException("PluginLoaderFactory.Create is not implemented.");
        }
    }
}
