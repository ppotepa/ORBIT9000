using System.Reflection;
using System.Runtime.Loader;

namespace ORBIT9000.Engine.IO.Loaders.PluginAssembly.Context
{
<<<<<<< HEAD
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
=======
    internal class PluginLoadContext(string pluginPath) : AssemblyLoadContext(isCollectible: true)
>>>>>>> bfa6c2d (Try fix pipeline)
    {
        private readonly AssemblyDependencyResolver _resolver = new(pluginPath);

        public Assembly LoadFromAssemblyBytes(byte[] assemblyBytes)
        {
<<<<<<< HEAD
            using var stream = new MemoryStream(assemblyBytes);
>>>>>>> e2b2b5a (Reworked Naming)
            return LoadFromStream(stream);
=======
            MemoryStream memoryStream = new(assemblyBytes);
            using MemoryStream stream = memoryStream;
            return this.LoadFromStream(stream);
>>>>>>> bfa6c2d (Try fix pipeline)
        }

        protected override Assembly Load(AssemblyName assemblyName)
<<<<<<< HEAD
<<<<<<< HEAD
        {
=======
        {            
>>>>>>> e2b2b5a (Reworked Naming)
=======
        {
>>>>>>> fd5a59f (Code Cleanup)
            Assembly? loadedAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(assembly => assembly.FullName == assemblyName.FullName);

            if (loadedAssembly != null)
            {
                return loadedAssembly;
            }

            string? assemblyPath = this._resolver.ResolveAssemblyToPath(assemblyName);

            if (assemblyPath != null)
            {
                return this.LoadFromAssemblyPath(assemblyPath);
            }

<<<<<<< HEAD
<<<<<<< HEAD
            return null!;
=======
            return null;
>>>>>>> e2b2b5a (Reworked Naming)
=======
            return null!;
>>>>>>> 53c6dc2 (Further Remove code smells.)
        }

        protected override nint LoadUnmanagedDll(string unmanagedDllName)
        {
            string? libraryPath = this._resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            if (libraryPath != null)
            {
                return this.LoadUnmanagedDllFromPath(libraryPath);
            }
            return nint.Zero;
        }
    }
}