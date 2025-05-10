using System.Threading.Channels;

namespace ORBIT9000.Core.Abstractions.Runtime
{
    public interface IMessageChannel<T>
    {
        ValueTask PublishAsync(T message);

        IAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken = default);
    }

    public class GlobalMessageChannel<T> : IMessageChannel<T>
    {
        private readonly Channel<T> _channel;

        public GlobalMessageChannel()
        {
            this._channel = Channel.CreateUnbounded<T>();
        }

        public async ValueTask PublishAsync(T message)
        {
            await this._channel.Writer.WriteAsync(message);
        }

        public IAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken = default)
        {
            return this._channel.Reader.ReadAllAsync(cancellationToken);
        }
    }
}