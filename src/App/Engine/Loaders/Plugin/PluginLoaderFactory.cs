using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Environment;
using ORBIT9000.Engine.Configuration.Raw;
using ORBIT9000.Engine.Loaders.Plugin.Implementations;
using ORBIT9000.Engine.Loaders.Plugin.Results;

namespace ORBIT9000.Engine.Loaders.Plugin
{
    /// <summary>
    /// Factory class for plugin loading strategies.
    /// </summary>
    internal static class PluginLoaderFactory
    {
        /// <summary>
        /// Resolves the appropriate plugin loading strategy based on configuration.
        /// </summary>
        /// <remarks>
        /// If plugins with explicitly defined paths are provided in <see cref="RawOrbitEngineConfig"/>,
        /// only those specified plugins will be loaded using <see cref="StringArrayPluginLoader"/>.
        /// Otherwise, plugins are loaded from the default directory using <see cref="DirectoryPluginLoader"/>.
        /// <para>
        /// <b> Active PluginInfo are Recommended for DEBUG and TESTING </b>
        /// </para>
        /// </remarks>
        /// <param name="rawPlugins">The raw configuration containing plugin settings.</param>
        /// <param name="logger">Optional logger for recording plugin loading operations.</param>
        /// <returns>An instance of <see cref="PluginLoadingStrategy"/> appropriate for the configuration.</returns>
        ///         
        public static IEnumerable<PluginLoadResult> Load(RawPluginInfo rawPlugins, ILogger? logger = default)
        {
            if (AppEnvironment.IsDebug)
            {
                if (rawPlugins.ActivePlugins.Length != 0)
                {
                    return new StringArrayPluginLoader(logger)
                        .AbortOnError(rawPlugins.AbortOnError)
                        .LoadPlugins(rawPlugins.ActivePlugins);
                }
                else
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
            }
            else
            {
                if (rawPlugins.ActivePlugins.Length != 0)
                {
                    return new StringArrayPluginLoader(logger)
                        .AbortOnError(rawPlugins.AbortOnError)
                        .LoadPlugins(rawPlugins.ActivePlugins);
                }
                else
                {
                    return new DirectoryPluginLoader(logger)
                        .AbortOnError(rawPlugins.AbortOnError)
                        .LoadPlugins(new DirectoryInfo("./Plugins"));
                }
            }
        }
    }
}
