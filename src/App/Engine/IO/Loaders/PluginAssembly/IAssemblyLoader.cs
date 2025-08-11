<<<<<<< HEAD
﻿
using ORBIT9000.Abstractions.Loaders;
=======
﻿using ORBIT9000.Core.Abstractions.Loaders;
>>>>>>> 254394d (Remove OverLogging)
using System.Reflection;

namespace ORBIT9000.Engine.IO.Loaders.PluginAssembly
{
<<<<<<< HEAD
    public interface IAssemblyLoader : IFileLoader<Assembly>
    {
=======
    public interface IAssemblyLoader : ILoader<Assembly>
<<<<<<< HEAD
    {        
>>>>>>> 254394d (Remove OverLogging)
=======
    {
>>>>>>> 83dd439 (Remove Code Smells)
        void UnloadAssembly(string assemblyPath);
        void UnloadAssembly(FileInfo info);
    }
}
