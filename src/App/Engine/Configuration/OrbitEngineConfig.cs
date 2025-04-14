
using System.Reflection;

namespace ORBIT9000.Engine.Configuration
{
    public class OrbitEngineConfig
    {
        public required Assembly[] Plugins { get; set; }
    }
}