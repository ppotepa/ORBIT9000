using Microsoft.Extensions.DependencyInjection;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Plugins.Example2.Services;

namespace ORBIT9000.Plugins.Example2
{
    public class Example2Plugin : IOrbitPlugin
    {
        public Task OnLoad()
        {
            return Task.CompletedTask;
        }

        public Task OnUnload()
        {
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<RandomNumberService>();
        }
    }   
  
}