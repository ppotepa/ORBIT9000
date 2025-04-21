using ORBIT9000.Engine;
using ORBIT9000.Plugins.Twitter;

namespace ORBIT9000.PoCDemo
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            OrbitEngine engine = new OrbitEngineBuilder()
                .UseConfiguration()
                .UseSerilogLogging()
                .RegisterPlugins()
                .Build();
            
            engine.Start();
       }
    }
}