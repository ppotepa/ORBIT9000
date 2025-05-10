using ORBIT9000.Core.Abstractions;

namespace ORBIT9000.Core.Attributes.Engine
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]

    public class SingletonAttribute : Attribute, IEngineAttribute
    {
        public SingletonAttribute(Type targetType)
        {
            if (targetType != typeof(IOrbitPlugin))
            {
                throw new InvalidOperationException("SingletonAttribute can only be used with the IOrbit implementations.");
            }
        }

        public SingletonAttribute()
        {
        }
    }
}
