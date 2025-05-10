using System.Reflection;
using System.Runtime.Loader;

namespace ORBIT9000.Engine.IO.Loaders.PluginAssembly.Context
{
    internal class PluginLoadContext(string pluginPath) : AssemblyLoadContext(isCollectible: true)
    {
        private readonly AssemblyDependencyResolver _resolver = new(pluginPath);

        public Assembly LoadFromAssemblyBytes(byte[] assemblyBytes)
        {
            MemoryStream memoryStream = new(assemblyBytes);
            using MemoryStream stream = memoryStream;
            return this.LoadFromStream(stream);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
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

            return null!;
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