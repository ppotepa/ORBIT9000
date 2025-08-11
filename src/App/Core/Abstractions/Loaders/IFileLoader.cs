<<<<<<< HEAD
﻿namespace ORBIT9000.Abstractions.Loaders
{
    public interface IFileLoader<TResult> : ILoader
    {
        TResult Load(FileInfo info, bool loadAsBinary = false);

        TResult? Load(FileInfo info, Func<TResult> scanner, bool loadAsBinary = false) => default;
    }

    public interface ILoader;
}
=======
﻿namespace ORBIT9000.Core.Abstractions.Loaders
{
    public interface IFileLoader<TResult> : ILoader
    {
        TResult Load(FileInfo info, bool loadAsBinary = false);

        TResult? Load(FileInfo info, Func<TResult> scanner, bool loadAsBinary = false) => default;
    }

<<<<<<< HEAD
    public interface ILoader
    {
    }
}
>>>>>>> b07db7d (Slightly Clean Up The BoilerPlate)
=======
    public interface ILoader;
}
>>>>>>> bfa6c2d (Try fix pipeline)
