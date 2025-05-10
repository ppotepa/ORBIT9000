namespace ORBIT9000.Engine
{
    public class PluginActivationInfo(bool registered)
    {
        #region Properties

        public bool AllowMultiple { get; internal set; } = false;
        public List<Task> Instances { get; set; } = [];
        public bool Registered { get; set; } = registered;

        #endregion Properties
    }
}