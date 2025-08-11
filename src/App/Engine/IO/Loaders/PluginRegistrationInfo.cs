<<<<<<< HEAD
<<<<<<< HEAD
﻿#nullable disable
namespace ORBIT9000.Engine.IO.Loaders
{
    public class PluginRegistrationInfo(bool registered)
    {
        public bool AllowMultiple { get; internal set; } = false;
        public bool IsLoaded { get; internal set; }
        public bool Registered { get; set; } = registered;
        public List<Task> Tasks { get; set; } = [];
        public Type Type { get; internal set; }
=======
﻿namespace ORBIT9000.Engine.IO.Loaders
=======
﻿#nullable disable
namespace ORBIT9000.Engine.IO.Loaders
>>>>>>> 9e426ec (Add LifetimeScope Sharing Between Plugins)
{
    public class PluginRegistrationInfo
    {
        public PluginRegistrationInfo(bool registered)
        {
            this.Registered = registered;
        }

        public bool AllowMultiple { get; internal set; } = false;
        public bool IsLoaded { get; internal set; }
        public bool Registered { get; set; }
        public List<Task> Tasks { get; set; } = new List<Task>();
        public Type Type { get; internal set;}
>>>>>>> e2b2b5a (Reworked Naming)
    }
}