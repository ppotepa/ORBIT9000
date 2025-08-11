<<<<<<< HEAD
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
=======
>>>>>>> fd5a59f (Code Cleanup)
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

>>>>>>> 914c644 (Add Pipe Handler)
        public PipeThreadHandler(EngineState state)
        {
            this._state = state ?? throw new ArgumentNullException(nameof(state));

            if (this._state.Engine is null)
            {
                throw new InvalidOperationException("Engine state is null.");
            }
        }

<<<<<<< HEAD
        #endregion Constructors

        #region Methods

        public async Task StartAsync()
        {
            ArgumentNullException.ThrowIfNull(this._state);
            ArgumentNullException.ThrowIfNull(this._state.Engine);

<<<<<<< HEAD
<<<<<<< HEAD
=======
        public async Task StartAsync()
        {
>>>>>>> 914c644 (Add Pipe Handler)
=======
>>>>>>> 86e317a (Refactor interfaces and improve null safety)
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
=======
            this._state.Engine.LogInformation("Starting PipeThread.");

            try
            {
                await using NamedPipeServerStream server = new("OrbitEngine", PipeDirection.Out);
>>>>>>> bfa6c2d (Try fix pipeline)

                await server.WaitForConnectionAsync();

                this._state.Engine.LogInformation("GUI Connected");

<<<<<<< HEAD
                var random = new Random();
>>>>>>> 914c644 (Add Pipe Handler)
=======
                Random random = new();
>>>>>>> bfa6c2d (Try fix pipeline)

                while (this._state.Engine.IsRunning)
                {
                    try
                    {
<<<<<<< HEAD
<<<<<<< HEAD
                        MessagePackSerializerOptions options = MessagePackSerializerOptions.Standard.WithResolver(CompositeResolver.Create(
=======
                        var options = MessagePackSerializerOptions.Standard.WithResolver(CompositeResolver.Create(
>>>>>>> 914c644 (Add Pipe Handler)
=======
                        MessagePackSerializerOptions options = MessagePackSerializerOptions.Standard.WithResolver(CompositeResolver.Create(
>>>>>>> bfa6c2d (Try fix pipeline)
                            ContractlessStandardResolver.Instance,
                            StandardResolver.Instance
                        ));

<<<<<<< HEAD
<<<<<<< HEAD
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
=======
                        var exampleData = RandomDataFiller.FillWithRandomData<ExampleData>();
>>>>>>> 1f2f8f4 (Improve Property Display)
=======
                        ExampleData exampleData = RandomDataFiller.FillWithRandomData<ExampleData>();
>>>>>>> bfa6c2d (Try fix pipeline)

                        byte[] buffer = MessagePackSerializer.Serialize(exampleData, options);

                        if (random.NextDouble() > 0.91 && server.IsConnected)
                        {
                            await server.WriteAsync(buffer);
                            this._state.Engine.LogInformation("Message sent to GUI. {A}", exampleData);
                        }
                        else
                        {
<<<<<<< HEAD
<<<<<<< HEAD
                            _state.Engine.LogDebug("Message skipped for this interval.");
>>>>>>> 914c644 (Add Pipe Handler)
=======
                            _state.Engine.LogInformation("Message skipped for this interval.");
>>>>>>> cd1f020 (Improve Logging and Naming)
=======
                            this._state.Engine.LogInformation("Message skipped for this interval.");
>>>>>>> bfa6c2d (Try fix pipeline)
                        }

                        await Task.Delay(TimeSpan.FromMilliseconds(random.Next(50, 200)));
                    }
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
                    catch (IOException ex)
=======
                    catch (System.IO.IOException ex)
>>>>>>> 7978821 (Enhance scheduling and error handling in engine components)
=======
                    catch (IOException ex)
>>>>>>> bfa6c2d (Try fix pipeline)
                    {
                        this._state.Engine.LogError("An error occurred while writing to the pipe: {Message}, Disposing...", ex.Message);
                        server.Disconnect();
                        await server.DisposeAsync();
                    }
                    catch (ObjectDisposedException ex)
<<<<<<< HEAD
=======
                    catch (Exception ex)
>>>>>>> 914c644 (Add Pipe Handler)
=======
>>>>>>> 7978821 (Enhance scheduling and error handling in engine components)
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
<<<<<<< HEAD

        #endregion Methods
=======
>>>>>>> 914c644 (Add Pipe Handler)
    }
}