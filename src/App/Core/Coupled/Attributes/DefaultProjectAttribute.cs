namespace ORBIT9000.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultProjectAttribute(string projectName) : Attribute
    {
        public string DefaultProjectName { get; } = projectName;
    }
}
