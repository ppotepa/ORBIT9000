using MessagePack;
using MessagePack.Resolvers;
using ORBIT9000.Core.Models.Pipe.ORBIT9000.Core.Models.Pipe;
using ORBIT9000.Engine.Runtime.State;
using System.IO.Pipes;
using TempTools;

namespace ORBIT9000.Engine.Strategies.Running
{
    internal class PipeThreadHandler
    {
        private readonly EngineState _state;

        public PipeThreadHandler(EngineState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));

            if (_state.Engine is null)
            {
                throw new InvalidOperationException("Engine state is null.");
            }
        }

        public async Task StartAsync()
        {
            _state.Engine.LogInformation("Starting PipeThread.");

            try
            {
                using var server = new NamedPipeServerStream("OrbitEngine", PipeDirection.Out);

                await server.WaitForConnectionAsync();

                _state.Engine.LogInformation("GUI Connected");

                var random = new Random();

                while (_state.Engine.IsRunning)
                {
                    try
                    {
                        var options = MessagePackSerializerOptions.Standard.WithResolver(CompositeResolver.Create(
                            ContractlessStandardResolver.Instance,
                            StandardResolver.Instance
                        ));

                        var exampleData = RandomDataFiller.FillWithRandomData<ExampleData>();

                        byte[] buffer = MessagePack.MessagePackSerializer.Serialize(exampleData, options);

                        if (random.NextDouble() > 0.91)
                        {
                            await server.WriteAsync(buffer, 0, buffer.Length);
                            _state.Engine.LogInformation("Message sent to GUI. {A}", exampleData);
                        }
                        else
                        {
                            _state.Engine.LogInformation("Message skipped for this interval.");
                        }

                        await Task.Delay(TimeSpan.FromMilliseconds(random.Next(50, 200)));
                    }
                    catch (Exception ex)
                    {
                        _state.Engine.LogError("An error occurred during the pipe thread loop: {Message}", ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _state.Engine.LogError("An error occurred in PipeThread: {Message}", ex.Message);
            }

            _state.Engine.LogInformation("PipeThread has completed.");
        }
    }
}