
namespace ORBIT9000.Engine
{
    public class PluginActivationInfo
    {
        #region Constructors

        public PluginActivationInfo(bool registered)
        {
            this.Registered = registered;
        }

        #endregion Constructors

        #region Properties

        public bool AllowMultiple { get; internal set; } = false;
        public List<Task> Instances { get; set; } = new List<Task>();
        public bool Registered { get; set; }

        #endregion Properties
    }
}