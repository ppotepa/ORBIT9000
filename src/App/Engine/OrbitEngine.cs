using ORBIT9000.Core.Plugin;
using ORBIT9000.Engine.Configuration;

namespace ORBIT9000.Engine
{
   
    public class OrbitEngine
    {
        private readonly OrbitEngineConfig config;

        public OrbitEngine(OrbitEngineConfig config) 
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            config.Plugins ??= [.. AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes()).Where(t => t.GetInterfaces().Contains(typeof(IOrbitPlugin<,>)))
                ]; 
        }

        public void Execute()
        {           
             var res = this.config.Plugins.Select(p => Activator.CreateInstance(p)).ToList();
        }
    }
}