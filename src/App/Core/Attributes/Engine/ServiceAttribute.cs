﻿namespace ORBIT9000.Core.Attributes.Engine
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute, IEngineAttribute
    {
        public ServiceAttribute()
        {
        }
    }
}