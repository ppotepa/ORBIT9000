using System.IO.Pipes;
using System.Text;

namespace Orbit9000.EngineTerminal
{
    protected class Program
    {
        protected static Program()
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

                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received Engine state: " + json);
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Pipe broken: " + ex.Message);
                    break;
                }
            }
        }
    }
}
