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
                await this._statusWriter.WriteAsync("Connected to engine.", cancellationToken);

                byte[] buffer = new byte[4096];

                while (!cancellationToken.IsCancellationRequested)
                {
                    int bytesRead = await client.ReadAsync(buffer, cancellationToken);
                    if (bytesRead == 0) break;

                    ExampleData data = MessagePackSerializer
                        .Deserialize<ExampleData>(new ReadOnlySequence<byte>(buffer, 0, bytesRead), options, cancellationToken);

                    await this._dataWriter.WriteAsync(data, cancellationToken);
                    await this._statusWriter.WriteAsync("New engine data received.", cancellationToken);
                    await Task.Delay(50, cancellationToken);
                }

                await this._statusWriter.WriteAsync("Pipe closed by server.", cancellationToken);
            }
            catch (Exception ex)
            {
                await this._statusWriter.WriteAsync($"Pipe error: {ex.Message}", cancellationToken);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this._dataWriter.Complete();
                    this._statusWriter.Complete();
                }

                base.Dispose(disposing);
                this.disposed = true;
            }
        }
    }
}