<<<<<<< HEAD
﻿namespace ORBIT9000.Abstractions.Providers
=======
﻿namespace ORBIT9000.Core.Abstractions.Providers
>>>>>>> bfa6c2d (Try fix pipeline)
{
    public interface IPluginProvider
    {
        IEnumerable<Type> Plugins { get; }

<<<<<<< HEAD
        Task<IOrbitPlugin> Activate(Type plugin, bool executeOnLoad = false);
        Task<IOrbitPlugin> Activate(object plugin, bool executeOnLoad = false);
=======
        Task<IOrbitPlugin> Activate(Type plugin);
        Task<IOrbitPlugin> Activate(object plugin);
>>>>>>> bfa6c2d (Try fix pipeline)

        void Unload(object plugin);
    }
}