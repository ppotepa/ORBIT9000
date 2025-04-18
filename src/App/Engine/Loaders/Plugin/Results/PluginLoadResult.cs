using System.Reflection;

namespace ORBIT9000.Engine.Loaders.Plugin.Results
{
    public class PluginLoadResult
    {
        private string Path;
        public bool FileExists;
        public bool IsDLL;
        public bool ContainsPlugins;
        public string[] Error;
        public Assembly? LoadedAssembly;
        public Type[] Plugins;

        public PluginLoadResult() { }

        public PluginLoadResult(string path, bool fileExists, bool isDll, 
            bool containsPlugins, string[] errors, Assembly? loadedAssembly, Type[] plugins)
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