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
<<<<<<< HEAD

=======
>>>>>>> 56ba6c0 (Add Generic Message Channel)
=======

>>>>>>> bfa6c2d (Try fix pipeline)
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
<<<<<<< HEAD
<<<<<<< HEAD
}
=======

}
>>>>>>> 56ba6c0 (Add Generic Message Channel)
=======
}
>>>>>>> bfa6c2d (Try fix pipeline)
