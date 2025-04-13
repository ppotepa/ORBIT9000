using ORBIT9000.Engine;
using ORBIT9000.Plugins.Tesla;

namespace ORBIT9000.PoCDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var a = new OrbitEngine
            (
                new() { Plugins = [typeof(TeslaPlugin)] }
            );

            a.Execute();
        }
    }
}
