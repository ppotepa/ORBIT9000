<<<<<<< HEAD:src/App/Core/Coupled/Attributes/Engine/SingletonAttribute.cs
﻿using ORBIT9000.Abstractions;
=======
﻿using ORBIT9000.Core.Abstractions;
>>>>>>> 9e426ec (Add LifetimeScope Sharing Between Plugins):src/App/Core/ORBIT9000.Core/Attributes/Engine/SingletonAttribute.cs

namespace ORBIT9000.Core.Attributes.Engine
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
<<<<<<< HEAD:src/App/Core/Coupled/Attributes/Engine/SingletonAttribute.cs
    public class SingletonAttribute : Attribute, IEngineAttribute
=======
    
    public class SingletonAttribute : Attribute
>>>>>>> 9e426ec (Add LifetimeScope Sharing Between Plugins):src/App/Core/ORBIT9000.Core/Attributes/Engine/SingletonAttribute.cs
    {
        public SingletonAttribute(Type targetType)
        {
            if (targetType != typeof(IOrbitPlugin))
            {
                throw new InvalidOperationException("SingletonAttribute can only be used with the IOrbit implementations.");
            }
        }
<<<<<<< HEAD:src/App/Core/Coupled/Attributes/Engine/SingletonAttribute.cs

        public SingletonAttribute()
        {
        }
    }
}
=======
    }
}
>>>>>>> 9e426ec (Add LifetimeScope Sharing Between Plugins):src/App/Core/ORBIT9000.Core/Attributes/Engine/SingletonAttribute.cs
