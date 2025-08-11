using System.Threading.Channels;

<<<<<<< HEAD
namespace ORBIT9000.Abstractions.Runtime
=======
namespace ORBIT9000.Core.Abstractions.Runtime
>>>>>>> 56ba6c0 (Add Generic Message Channel)
{
    public interface IMessageChannel<T>
    {
        ValueTask PublishAsync(T message);
<<<<<<< HEAD

=======
>>>>>>> 56ba6c0 (Add Generic Message Channel)
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
<<<<<<< HEAD
}
=======

}
>>>>>>> 56ba6c0 (Add Generic Message Channel)
