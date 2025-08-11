using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Environment;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Loaders.Plugin.Strategies;
using ORBIT9000.Engine.Loaders.Plugin.Results;

namespace ORBIT9000.Engine.Loaders.Plugin
{
    internal static class PluginLoaderFactory
    {
        public static IEnumerable<PluginLoadResult> Load(RawPluginInfo rawPlugins, ILogger? logger = default)
        {
            if (rawPlugins.ActivePlugins.Length != 0)
            {
                return LoadFromActivePlugins(rawPlugins, logger);
            }

            return AppEnvironment.IsDebug
                ? LoadFromDebugDirectory(rawPlugins, logger)
                : LoadFromDefaultDirectory(rawPlugins, logger);
        }

        private static IEnumerable<PluginLoadResult> LoadFromActivePlugins(RawPluginInfo rawPlugins, ILogger? logger)
        {
            return new StringArrayPluginLoader(logger)
                .AbortOnError(rawPlugins.AbortOnError)
                .LoadPlugins(rawPlugins.ActivePlugins);
        }

        private static IEnumerable<PluginLoadResult> LoadFromDebugDirectory(RawPluginInfo rawPlugins, ILogger? logger)
        {
            DirectoryInfo? directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (directory != null)
            {
                string srcPath = Path.Combine(directory.FullName, "src");
                if (Directory.Exists(srcPath))
                {
                    return new DebugDirectoryPluginLoader(logger)
                        .AbortOnError(rawPlugins.AbortOnError)
                        .LoadPlugins(new DirectoryInfo(srcPath));
                }

                directory = directory.Parent;
            }

            throw new FileNotFoundException("src directory not found. Please run the application from the root of the repository.");
        }

        private static IEnumerable<PluginLoadResult> LoadFromDefaultDirectory(RawPluginInfo rawPlugins, ILogger? logger)
        {
            return new DirectoryPluginLoader(logger)
                .AbortOnError(rawPlugins.AbortOnError)
                .LoadPlugins(new DirectoryInfo("./Plugins"));
        }
    }
}
