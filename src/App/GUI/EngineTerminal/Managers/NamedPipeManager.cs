<<<<<<< HEAD
﻿using EngineTerminal.Contracts;
using MessagePack;
using MessagePack.Resolvers;
using ORBIT9000.Core.Environment;
using ORBIT9000.Core.Models.Pipe;
using System.Buffers;
using System.IO.Pipes;
using System.Threading.Channels;

namespace EngineTerminal.Managers
{
    public class NamedPipeManager(
        Channel<ExampleData> dataChannel,
        Channel<string> statusChannel,
        string serverName,
        string pipeName) : Disposable, IPipeManager
    {
        private readonly ChannelWriter<ExampleData> _dataWriter = dataChannel.Writer;
        private readonly ChannelWriter<string> _statusWriter = statusChannel.Writer;

        public async Task StartProcessingAsync(CancellationToken cancellationToken)
        {
            await using NamedPipeClientStream client = new(serverName, pipeName, PipeDirection.In);
            MessagePackSerializerOptions options = MessagePackSerializerOptions.Standard
                .WithResolver(CompositeResolver.Create(
                    ContractlessStandardResolver.Instance,
                    StandardResolver.Instance));

            try
            {
                await client.ConnectAsync(cancellationToken);
                await _statusWriter.WriteAsync("Connected to engine.", cancellationToken);

                byte[] buffer = new byte[4096];

                while (!cancellationToken.IsCancellationRequested)
                {
                    int bytesRead = await client.ReadAsync(buffer, cancellationToken);
                    if (bytesRead == 0) break;

                    ExampleData data = MessagePackSerializer
                        .Deserialize<ExampleData>(new ReadOnlySequence<byte>(buffer, 0, bytesRead), options, cancellationToken);

                    await _dataWriter.WriteAsync(data, cancellationToken);
                    await _statusWriter.WriteAsync("New engine data received.", cancellationToken);
                    await Task.Delay(50, cancellationToken);
                }

                await _statusWriter.WriteAsync("Pipe closed by server.", cancellationToken);
            }
            catch (Exception ex)
            {
                await _statusWriter.WriteAsync($"Pipe error: {ex.Message}", cancellationToken);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _dataWriter.Complete();
                    _statusWriter.Complete();
                }

                base.Dispose(disposing);
                disposed = true;
            }
        }
    }
}
=======
﻿using EngineTerminal.EventArgs;
using MessagePack;
using MessagePack.Resolvers;
using Orbit9000.EngineTerminal;
using Orbit9000.EngineTerminal.EventArgs;
using ORBIT9000.Core.Models.Pipe;
using System.Buffers;
using System.IO.Pipes;

/// <summary>
/// This is an experimental terminal project for the Orbit9000 engine.  
/// It is designed with minimal dependencies and libraries to focus on core functionality.  
/// The primary focus is to create pipe communication and generic property change handling for better
/// display and monitoring.
/// </summary>
namespace EngineTerminal.Managers
{
    public class NamedPipeManager : IDisposable
    {
        private readonly string _pipeName;
        private readonly string _serverName;
        private NamedPipeClientStream _pipeClient;

        public NamedPipeManager(string serverName, string pipeName)
        {
            _serverName = serverName;
            _pipeName = pipeName;
        }

        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        public void Dispose()
        {
            _pipeClient?.Dispose();
        }

        public void StartProcessing(CancellationToken cancellationToken)
        {
            Task.Run(() => ProcessDataFromPipe(cancellationToken));
        }

        private void OnDataReceived(ExampleData data)
        {
            DataReceived?.Invoke(this, new DataReceivedEventArgs(data));
        }

        private void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(this, new StatusChangedEventArgs(status));
        }

        private async Task ProcessDataFromPipe(CancellationToken cancellationToken)
        {
            _pipeClient = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.In);

            var options = MessagePackSerializerOptions.Standard.WithResolver(
                CompositeResolver.Create(
                    ContractlessStandardResolver.Instance,
                    StandardResolver.Instance
                )
            );

            try
            {
                
                OnStatusChanged("Connecting to engine...");

                await _pipeClient.ConnectAsync(cancellationToken);

                OnStatusChanged("Connected to engine!");

                var buffer = new byte[4096];
                OnStatusChanged("Awaiting data...");
                
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        int bytesRead = await _pipeClient.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                        if (bytesRead == 0)
                        {
                            OnStatusChanged("Server closed connection.");
                            break;
                        }
                        
                        var receivedData = MessagePackSerializer.Deserialize<ExampleData>(
                            new ReadOnlySequence<byte>(buffer, 0, bytesRead),
                            options
                        );
                        
                        OnDataReceived(receivedData);
                        OnStatusChanged($"Received Engine state update {new Random().Next(1, 100)}");
                    }
                    catch (IOException ex)
                    {
                        OnStatusChanged($"Pipe broken: {ex.Message}");
                        break;
                    }
                    catch (Exception ex)
                    {
                        OnStatusChanged($"Error processing data: {ex.Message}");
                    }

                    await Task.Delay(50, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error connecting to pipe: {ex.Message}");
            }
        }
    }
}
>>>>>>> 80f2a0e (Split Responsibilities To Managers)
