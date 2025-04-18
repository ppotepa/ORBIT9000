
namespace ORBIT9000.Engine
{
    internal class PluginActivationInfo
    {
        public PluginActivationInfo(bool registered, Type item)
        {
            this.Registered = registered;
            this.Item = item;
        }

        public bool Registered { get; set; }
        public Type Item { get; private set; }
    }
}