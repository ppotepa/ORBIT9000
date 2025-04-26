using MessagePack;
using ORBIT9000.Engine.Configuration;
using System.Buffers;
using System.IO.Pipes;

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
