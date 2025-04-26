namespace ORBIT9000.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultProjectAttribute : Attribute
    {
        public DefaultProjectAttribute(string projectName)
        {
            this.DefaultProjectName = projectName;
        }

        public string DefaultProjectName { get; }
    }
}
