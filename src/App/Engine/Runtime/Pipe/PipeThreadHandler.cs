<<<<<<< HEAD
using MessagePack;
using MessagePack.Resolvers;
using ORBIT9000.Core.Models.Pipe;
using ORBIT9000.Core.TempTools;
using ORBIT9000.Engine.Runtime.State;
using System.IO.Pipes;

namespace ORBIT9000.Engine.Runtime.Pipe
{
    internal class PipeThreadHandler
    {
        #region Fields

        private readonly EngineState _state;

        #endregion Fields

        #region Constructors

=======
using MessagePack.Resolvers;
using MessagePack;
using ORBIT9000.Engine.Runtime.State;
using System.IO.Pipes;
using ORBIT9000.Core.Models.Pipe;

namespace ORBIT9000.Engine.Strategies.Running
{
    internal class PipeThreadHandler
    {
        private readonly EngineState _state;

>>>>>>> 914c644 (Add Pipe Handler)
        public PipeThreadHandler(EngineState state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));

            if (_state.Engine is null)
            {
                throw new InvalidOperationException("Engine state is null.");
            }
        }

<<<<<<< HEAD
        #endregion Constructors

        #region Methods

        public async Task StartAsync()
        {
            ArgumentNullException.ThrowIfNull(_state);
            ArgumentNullException.ThrowIfNull(_state.Engine);

=======
        public async Task StartAsync()
        {
>>>>>>> 914c644 (Add Pipe Handler)
            _state.Engine.LogInformation("Starting PipeThread.");

            try
            {
<<<<<<< HEAD
                await using NamedPipeServerStream server = new("OrbitEngine", PipeDirection.Out);

                await server.WaitForConnectionAsync();

                _state.Engine.LogInformation("GUI Connected");

                Random random = new();
=======
                using var server = new NamedPipeServerStream("OrbitEngine", PipeDirection.Out);

                await server.WaitForConnectionAsync();
                _state.Engine.LogInformation("GUI Connected");

                var random = new Random();
>>>>>>> 914c644 (Add Pipe Handler)

                while (_state.Engine.IsRunning)
                {
                    try
                    {
<<<<<<< HEAD
                        MessagePackSerializerOptions options = MessagePackSerializerOptions.Standard.WithResolver(CompositeResolver.Create(
=======
                        var options = MessagePackSerializerOptions.Standard.WithResolver(CompositeResolver.Create(
>>>>>>> 914c644 (Add Pipe Handler)
                            ContractlessStandardResolver.Instance,
                            StandardResolver.Instance
                        ));

<<<<<<< HEAD
                        ExampleData exampleData = RandomDataFiller.FillWithRandomData<ExampleData>();

                        byte[] buffer = MessagePackSerializer.Serialize(exampleData, options);

                        if (random.NextDouble() > 0.91 && server.IsConnected)
                        {
                            await server.WriteAsync(buffer);
                            _state.Engine.LogInformation("Message sent to GUI. {A}", exampleData);
                        }
                        else
                        {
                            _state.Engine.LogInformation("Message skipped for this interval.");
=======
                        var exampleData = new ExampleData
                        {
                            Frame1 = new SettingsData
                            {
                                Setting1 = random.Next(1, 100),
                                Setting2 = random.Next(1, 100).ToString(),
                            },

                            Frame2 = new EngineData
                            {
                                Setting1 = random.Next(1, 100),
                                Setting2 = random.Next(1, 100)
                            }
                        };

                        byte[] buffer = MessagePack.MessagePackSerializer.Serialize(exampleData, options);

                        if (random.NextDouble() > 0.97)
                        {
                            await server.WriteAsync(buffer, 0, buffer.Length);
                            _state.Engine.LogDebug("Message sent to GUI.");
                        }
                        else
                        {
                            _state.Engine.LogDebug("Message skipped for this interval.");
>>>>>>> 914c644 (Add Pipe Handler)
                        }

                        await Task.Delay(TimeSpan.FromMilliseconds(random.Next(50, 200)));
                    }
<<<<<<< HEAD
                    catch (IOException ex)
                    {
                        _state.Engine.LogError("An error occurred while writing to the pipe: {Message}, Disposing...", ex.Message);
                        server.Disconnect();
                        await server.DisposeAsync();
                    }
                    catch (ObjectDisposedException ex)
=======
                    catch (Exception ex)
>>>>>>> 914c644 (Add Pipe Handler)
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
<<<<<<< HEAD

        #endregion Methods
=======
>>>>>>> 914c644 (Add Pipe Handler)
    }
}