
using ORBIT9000.Abstractions.Loaders;
using System.Reflection;

namespace ORBIT9000.Engine.IO.Loaders.PluginAssembly
{
    public interface IAssemblyLoader : IFileLoader<Assembly>
    {
        void UnloadAssembly(string assemblyPath);
        void UnloadAssembly(FileInfo info);
    }
}
