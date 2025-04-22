using SystemAssembly = System.Reflection.Assembly;

namespace ORBIT9000.Engine.Loaders.Plugin.Results
{
    public class PluginLoadResult
    {
        public PluginLoadResult(
            string path,
            bool fileExists,
            bool isDll,
            bool containsPlugins,
            string[] errors,
            SystemAssembly? loadedAssembly,
            Type[] plugins)
        {
            ContainsPlugins = containsPlugins;
            Error = errors;
            FileExists = fileExists;
            IsDLL = isDll;
            LoadedAssembly = loadedAssembly;
            Path = path;
            Plugins = plugins;
        }

        public bool ContainsPlugins { get; }
        public string[] Error { get; }
        public bool FileExists { get; }
        public bool IsDLL { get; }
        public SystemAssembly? LoadedAssembly { get; }
        public string Path { get; }
        public Type[] Plugins { get; }
    }
}
