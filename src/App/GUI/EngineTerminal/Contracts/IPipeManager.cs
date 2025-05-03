namespace EngineTerminal.Contracts
{
    public interface IPipeManager
    {
        Task StartProcessingAsync(CancellationToken cancellationToken);
    }
}