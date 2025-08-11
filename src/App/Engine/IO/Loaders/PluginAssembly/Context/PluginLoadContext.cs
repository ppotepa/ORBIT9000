using System.Reflection;
using System.Runtime.Loader;

namespace ORBIT9000.Engine.IO.Loaders.PluginAssembly.Context
{
<<<<<<< HEAD
    internal class PluginLoadContext(string pluginPath) : AssemblyLoadContext(isCollectible: true)
    {
        private readonly AssemblyDependencyResolver _resolver = new(pluginPath);

        public Assembly LoadFromAssemblyBytes(byte[] assemblyBytes)
        {
            MemoryStream memoryStream = new(assemblyBytes);
            using MemoryStream stream = memoryStream;
=======
    internal class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        public Assembly LoadFromAssemblyBytes(byte[] assemblyBytes)
        {
            using var stream = new MemoryStream(assemblyBytes);
>>>>>>> e2b2b5a (Reworked Naming)
            return LoadFromStream(stream);
        }

        protected override Assembly Load(AssemblyName assemblyName)
<<<<<<< HEAD
        {
=======
        {            
>>>>>>> e2b2b5a (Reworked Naming)
            Assembly? loadedAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(assembly => assembly.FullName == assemblyName.FullName);

            if (loadedAssembly != null)
            {
                return loadedAssembly;
            }

            string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);

            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

<<<<<<< HEAD
            return null!;
=======
            return null;
>>>>>>> e2b2b5a (Reworked Naming)
        }

        protected override nint LoadUnmanagedDll(string unmanagedDllName)
        {
            string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }
            return nint.Zero;
        }
    }
}