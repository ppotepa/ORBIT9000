using EngineTerminal.Contracts;
using EngineTerminal.Managers;
using Microsoft.Extensions.DependencyInjection;
using ORBIT9000.Core.Models.Pipe;
using ORBIT9000.Engine;
using System.Threading.Channels;

namespace Orbit9000.EngineTerminal
{
    public static class Program
    {
        #region Methods

        public static async Task Main(string[] args)
        {
            var dataChannel = Channel.CreateUnbounded<ExampleData>();
            var statusChannel = Channel.CreateUnbounded<string>();

            var services = new ServiceCollection();

            services.AddSingleton(dataChannel);
            services.AddSingleton(statusChannel);

            services.AddSingleton<IDataManager, DataManager>();
            services.AddSingleton<IUIManager, UIManager>();

            services.AddSingleton<IPipeManager>(provider =>
            {
                var dataChannel = provider.GetRequiredService<Channel<ExampleData>>();
                var propertyChannel = provider.GetRequiredService<Channel<string>>();

                return new NamedPipeManager(dataChannel, propertyChannel, ".", nameof(OrbitEngine));
            });

            services.AddSingleton<ApplicationController>();

            var provider = services.BuildServiceProvider();
            var app = provider.GetRequiredService<ApplicationController>();

            await app.RunAsync();
        }

        #endregion Methods
    }
}