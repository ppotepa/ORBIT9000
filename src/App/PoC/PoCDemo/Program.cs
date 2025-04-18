using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Engine;
using ORBIT9000.Engine.Extensions;
using Serilog;

namespace ORBIT9000.PoCDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            OrbitEngine engine = new OrbitEngine();
            engine.Start();
       }
    }
}