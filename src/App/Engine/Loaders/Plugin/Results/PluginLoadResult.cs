using SystemAssembly = System.Reflection.Assembly;

namespace ORBIT9000.Engine.Loaders.Plugin.Results
{
    public class PluginLoadResult
    {
        public bool ContainsPlugins;
        public string[] Error;
        public bool FileExists;
        public bool IsDLL;
        public SystemAssembly? LoadedAssembly;
        public Type[] Plugins;
        private string Path;

        public PluginLoadResult(string path, bool fileExists, bool isDll, 
            bool containsPlugins, string[] errors, SystemAssembly? loadedAssembly, Type[] plugins)
        {
            this.Path = path;
            this.FileExists = fileExists;
            this.IsDLL = isDll;
            this.ContainsPlugins = containsPlugins;
            this.Error = errors;
            this.LoadedAssembly = loadedAssembly;          
            this.Plugins = plugins;
        }
    }
}