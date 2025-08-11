<<<<<<< HEAD
<<<<<<< HEAD
﻿using EngineTerminal.Contracts;
using EngineTerminal.Managers;
using Microsoft.Extensions.DependencyInjection;
using ORBIT9000.Core.Models.Pipe;
using ORBIT9000.Engine;
using System.Threading.Channels;

namespace EngineTerminal
{
    public static class Program
    {
        #region Methods

        public static async Task Main(string[] args)
        {
            if (args is not null)
            {
                Channel<ExampleData> dataChannel = Channel.CreateUnbounded<ExampleData>();
                Channel<string> statusChannel = Channel.CreateUnbounded<string>();

                ServiceCollection services = new();

                services.AddSingleton(dataChannel);
                services.AddSingleton(statusChannel);

                services.AddSingleton<IDataManager, DataManager>();
                services.AddSingleton<IUIManager, UIManager>();

                services.AddSingleton<IPipeManager>(provider =>
                {
                    Channel<ExampleData> dataChannel = provider.GetRequiredService<Channel<ExampleData>>();
                    Channel<string> propertyChannel = provider.GetRequiredService<Channel<string>>();

                    return new NamedPipeManager(dataChannel, propertyChannel, ".", nameof(OrbitEngine));
                });

                services.AddSingleton<ApplicationController>();

                ServiceProvider provider = services.BuildServiceProvider();
                ApplicationController app = provider.GetRequiredService<ApplicationController>();

                await app.RunAsync();
            }
            else
            {
                throw new ArgumentNullException(nameof(args));
            }
        }

        #endregion Methods
    }
}
=======
﻿using System.IO.Pipes;
using System.Text;
=======
﻿using MessagePack;
using ORBIT9000.Engine.Configuration;
using System.Buffers;
using System.IO.Pipes;
>>>>>>> 1aafd5a (Add Basic Messaging)

namespace Orbit9000.EngineTerminal
{
    static class Program
    {
        static Program()
        {
        }

        static async Task Main(string[] args)
        {
            var client = new NamedPipeClientStream(".", "OrbitEngine", PipeDirection.In);
            await client.ConnectAsync();
            Console.WriteLine("Connected to engine!");

            var buffer = new byte[4096];

            while (true)
            {
                try
                {
                    int bytesRead = await client.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Server closed connection.");
                        break;
                    }

                    var @object = MessagePackSerializer.Deserialize<List<PluginInfo>>(new ReadOnlySequence<byte>(buffer));
                    Console.WriteLine("Received Engine state: " + @object.Count);

                    foreach(var pluginInfo in @object)
                    {
                        Console.WriteLine($"Plugin: {pluginInfo.PluginType}, Activated: {pluginInfo.Activated}");
                    }   
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Pipe broken: " + ex.Message);
                    break;
                }

                Thread.Sleep(1000);
            }
        }
    }
}
>>>>>>> 590e002 (Add Temporary NamedPipe and Receiving Console App)
