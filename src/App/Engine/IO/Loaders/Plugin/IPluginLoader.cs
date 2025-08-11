<<<<<<< HEAD
<<<<<<< HEAD
﻿using ORBIT9000.Engine.Configuration;
=======
﻿using ORBIT9000.Engine.Loaders.Plugin.Results;
>>>>>>> e2b2b5a (Reworked Naming)
=======
﻿using ORBIT9000.Engine.Configuration;
>>>>>>> 254394d (Remove OverLogging)

namespace ORBIT9000.Engine.IO.Loaders.Plugin
{
    public interface IPluginLoader<in TSource> : IPluginLoader
        where TSource : class
    {
<<<<<<< HEAD
<<<<<<< HEAD
        IEnumerable<PluginInfo> LoadPlugins(TSource source);
=======
        IEnumerable<AssemblyLoadResult> LoadPlugins(TSource source);
>>>>>>> e2b2b5a (Reworked Naming)
=======
        IEnumerable<PluginInfo> LoadPlugins(TSource source);
>>>>>>> 254394d (Remove OverLogging)
    }

    public interface IPluginLoader
    {
<<<<<<< HEAD
<<<<<<< HEAD
        IEnumerable<PluginInfo> LoadPlugins(object source);
        void Unload(object plugin);
=======
        IEnumerable<AssemblyLoadResult> LoadPlugins(object source);
>>>>>>> e2b2b5a (Reworked Naming)
=======
        IEnumerable<PluginInfo> LoadPlugins(object source);
>>>>>>> 254394d (Remove OverLogging)
    }
}
