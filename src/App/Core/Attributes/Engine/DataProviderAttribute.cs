﻿namespace ORBIT9000.Core.Attributes.Engine
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataProviderAttribute : Attribute, IEngineAttribute
    {
        public DataProviderAttribute()
        {
        }
    }
}
