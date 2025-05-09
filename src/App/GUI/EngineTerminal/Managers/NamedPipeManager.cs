using EngineTerminal.Contracts;
using MessagePack;
using MessagePack.Resolvers;
using ORBIT9000.Core.Environment;
using ORBIT9000.Core.Models.Pipe.ORBIT9000.Core.Models.Pipe;
using System.Buffers;
using System.IO.Pipes;
using System.Threading.Channels;

namespace EngineTerminal.Managers
{
    public class NamedPipeManager : Disposable, IPipeManager
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