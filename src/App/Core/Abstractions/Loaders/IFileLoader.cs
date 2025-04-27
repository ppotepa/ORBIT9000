namespace ORBIT9000.Core.Abstractions.Loaders
{

    public interface IFileLoader<TResult> : ILoader
    {
        TResult Load(FileInfo info, bool loadAsBinary = false);
        TResult? Load (FileInfo info, Func<TResult> scanner, bool loadAsBinary = false) => default;     
    }

    public interface ILoader
    {
    }
}
