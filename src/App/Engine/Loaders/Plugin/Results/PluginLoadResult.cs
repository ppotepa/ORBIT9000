using System.Reflection;

namespace ORBIT9000.Engine.Loaders.Plugin.Results
{
    public class PluginLoadResult
    {
        private string path;
        public bool fileExists;
        public bool isDll;
        public bool containsPlugins;
        public string[] errors;
        public Assembly? loadedAssembly;

        public PluginLoadResult() { }

        public PluginLoadResult(string path, bool fileExists, bool isDll, bool containsPlugins, string[] errors, Assembly? loadedAssembly)
        {
            this.path = path;
            this.fileExists = fileExists;
            this.isDll = isDll;
            this.containsPlugins = containsPlugins;
            this.errors = errors;
            this.loadedAssembly = loadedAssembly;
        }
    }
}