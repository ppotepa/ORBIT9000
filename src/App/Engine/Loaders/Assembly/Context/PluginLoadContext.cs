using System.Reflection;
using System.Runtime.Loader;

using SystemAssembly = System.Reflection.Assembly;

namespace ORBIT9000.Engine.Loaders.Assembly.Context
{
    // Custom AssemblyLoadContext for plugin isolation
    internal class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        // Load assembly from bytes
        public SystemAssembly LoadFromAssemblyBytes(byte[] assemblyBytes)
        {
            using var stream = new MemoryStream(assemblyBytes);
            return LoadFromStream(stream);
        }

        protected override SystemAssembly Load(AssemblyName assemblyName)
        {
            // Try to resolve the assembly path using the dependency resolver
            string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            // If not found through the resolver, return null to let the default context handle it
            return null;
        }

        protected override nint LoadUnmanagedDll(string unmanagedDllName)
        {
            // Try to resolve native library path using the dependency resolver
            string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            // If not found through the resolver, return IntPtr.Zero to let the default context handle it
            return nint.Zero;
        }
    }
}