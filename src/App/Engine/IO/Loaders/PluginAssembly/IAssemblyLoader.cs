using ORBIT9000.Core.Abstractions.Loaders;
using System.Reflection;

namespace ORBIT9000.Engine.IO.Loaders.PluginAssembly
{
    public interface IAssemblyLoader : ILoader<Assembly>
    {        
        void UnloadAssembly(string assemblyPath);
        void UnloadAssembly(FileInfo info);
    }
}
