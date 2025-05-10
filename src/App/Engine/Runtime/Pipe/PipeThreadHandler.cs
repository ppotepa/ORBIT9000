using MessagePack;
using MessagePack.Resolvers;
using ORBIT9000.Core.Models.Pipe.ORBIT9000.Core.Models.Pipe;
using ORBIT9000.Core.TempTools;
using ORBIT9000.Engine.Runtime.State;
using System.IO.Pipes;

namespace ORBIT9000.Engine.Runtime.Pipe
{
    internal class PipeThreadHandler
    {
        private readonly EngineState _state;

        public PipeThreadHandler(EngineState state)
        {
            this._state = state ?? throw new ArgumentNullException(nameof(state));

            if (this._state.Engine is null)
            {
                throw new InvalidOperationException("Engine state is null.");
            }
        }

        public async Task StartAsync()
        {
            ArgumentNullException.ThrowIfNull(this._state);
            ArgumentNullException.ThrowIfNull(this._state.Engine);

            this._state.Engine.LogInformation("Starting PipeThread.");

            try
            {
                await using NamedPipeServerStream server = new("OrbitEngine", PipeDirection.Out);

                await server.WaitForConnectionAsync();

                this._state.Engine.LogInformation("GUI Connected");

                Random random = new();

                while (this._state.Engine.IsRunning)
                {
                    try
                    {
                        MessagePackSerializerOptions options = MessagePackSerializerOptions.Standard.WithResolver(CompositeResolver.Create(
                            ContractlessStandardResolver.Instance,
                            StandardResolver.Instance
                        ));

                        ExampleData exampleData = RandomDataFiller.FillWithRandomData<ExampleData>();

                        byte[] buffer = MessagePackSerializer.Serialize(exampleData, options);

                        if (random.NextDouble() > 0.91 && server.IsConnected)
                        {
                            await server.WriteAsync(buffer);
                            this._state.Engine.LogInformation("Message sent to GUI. {A}", exampleData);
                        }
                        else
                        {
                            this._state.Engine.LogInformation("Message skipped for this interval.");
                        }

                        await Task.Delay(TimeSpan.FromMilliseconds(random.Next(50, 200)));
                    }
                    catch (IOException ex)
                    {
                        this._state.Engine.LogError("An error occurred while writing to the pipe: {Message}, Disposing...", ex.Message);
                        server.Disconnect();
                        await server.DisposeAsync();
                    }
                    catch (ObjectDisposedException ex)
                    {
                        this._state.Engine.LogError("An error occurred during the pipe thread loop: {Message}", ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                this._state.Engine.LogError("An error occurred in PipeThread: {Message}", ex.Message);
            }

            this._state.Engine.LogInformation("PipeThread has completed.");
        }
    }
}