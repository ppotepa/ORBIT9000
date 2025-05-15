using System.Threading.Channels;

namespace ORBIT9000.Abstractions.Runtime
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
            _channel = Channel.CreateUnbounded<T>();
        }

        public async ValueTask PublishAsync(T message)
        {
            await _channel.Writer.WriteAsync(message);
        }

        public IAsyncEnumerable<T> ReadAllAsync(CancellationToken cancellationToken = default)
        {
            return _channel.Reader.ReadAllAsync(cancellationToken);
        }
    }
}