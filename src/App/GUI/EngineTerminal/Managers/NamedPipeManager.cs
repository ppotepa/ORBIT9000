<<<<<<< HEAD
<<<<<<< HEAD
﻿using EngineTerminal.Contracts;
using MessagePack;
using MessagePack.Resolvers;
<<<<<<< HEAD
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
=======
﻿using EngineTerminal.Contracts;
>>>>>>> 5ae5b98 (Add Inversion of Control)
using MessagePack;
using MessagePack.Resolvers;
using ORBIT9000.Core.Models.Pipe;
=======
using ORBIT9000.Core.Models.Pipe.ORBIT9000.Core.Models.Pipe;
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
using System.Buffers;
using System.IO.Pipes;
using System.Threading.Channels;

namespace EngineTerminal.Managers
{
    public class NamedPipeManager : IPipeManager, IDisposable
    {
        private readonly ChannelWriter<ExampleData> _dataWriter;
        private readonly ChannelWriter<string> _statusWriter;
        private readonly string _serverName;
        private readonly string _pipeName;

        public NamedPipeManager(
            Channel<ExampleData> dataChannel,
            Channel<string> statusChannel,
            string serverName,
            string pipeName)
        {
            _dataWriter = dataChannel.Writer;
            _statusWriter = statusChannel.Writer;
            _serverName = serverName;
            _pipeName = pipeName;
        }

        public async Task StartProcessingAsync(CancellationToken cancellationToken)
        {
            using var client = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.In);
            MessagePackSerializerOptions options = MessagePackSerializerOptions.Standard
                .WithResolver(CompositeResolver.Create(
                    ContractlessStandardResolver.Instance,
                    StandardResolver.Instance));

            try
            {
                await client.ConnectAsync(cancellationToken);
                await _statusWriter.WriteAsync("Connected to engine.", cancellationToken);

                var buffer = new byte[4096];

                while (!cancellationToken.IsCancellationRequested)
                {
                    var bytesRead = await client.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (bytesRead == 0) break;

                    var data = MessagePackSerializer.Deserialize<ExampleData>(
                        new ReadOnlySequence<byte>(buffer, 0, bytesRead), options);

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

        public void Dispose()
        { }
    }
<<<<<<< HEAD
}
>>>>>>> 80f2a0e (Split Responsibilities To Managers)
=======
}
>>>>>>> 5ae5b98 (Add Inversion of Control)
