namespace ORBIT9000.Engine.Loaders.Assembly
{
    internal class AssemblyLoadResult
    {
        public AssemblyLoadResult(System.Reflection.Assembly? loadedAssembly, bool containsPlugins, Type[] plugins, List<Exception> exceptions)
        {
            LoadedAssembly = loadedAssembly;
            ContainsPlugins = containsPlugins;
            Plugins = plugins;
            Exceptions = exceptions;
        }

        public bool ContainsPlugins { get; }
        public System.Reflection.Assembly? LoadedAssembly { get; }
        public Type[] Plugins { get; }
        public List<Exception> Exceptions { get; } = [];
    }
}
