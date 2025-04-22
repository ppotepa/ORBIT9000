using System.Reflection;

namespace ORBIT9000.Engine.Loaders.Plugin.Results
{
    public class AssemblyLoadResult
    {
        public AssemblyLoadResult(Assembly? assembly, Type[] pluginTypes, List<Exception> exceptions, bool exists, bool isDll, bool containsPlugins, string error)
        {
            this.Assembly = assembly;
            this.PluginTypes = pluginTypes;
            this.Exceptions = exceptions;
            this.FileExists = exists;
            this.IsDll = isDll;
            this.LoadedAssembly = assembly;
            this.ContainsPlugins = PluginTypes.Any();
        }

        public readonly Assembly? Assembly;        
        public readonly string[] Error;
        public readonly IEnumerable<Exception>? Exceptions;
        public readonly bool FileExists;
        public readonly bool IsDll;
        public readonly Assembly LoadedAssembly;
        public readonly bool ContainsPlugins;
        public readonly string Path;
        public readonly Type[] PluginTypes;
    }

    public class TryLoadAssemblyResult
    {
        public TryLoadAssemblyResult(Assembly? assembly, Type[] pluginTypes, List<Exception> exceptions)
        {
            this.Assembly = assembly;
            this.PluginTypes = pluginTypes;
            this.Exceptions = exceptions;
        }

        public readonly Assembly? Assembly;
        public readonly Type[] PluginTypes;
        public readonly List<Exception> Exceptions;
    }
}