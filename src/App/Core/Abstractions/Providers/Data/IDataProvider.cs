<<<<<<< HEAD
﻿using ORBIT9000.Abstractions.Data.Entities;

namespace ORBIT9000.Abstractions.Providers.Data
{
    public interface IDataProvider;
    public interface IDataProvider<TResult> : IDataProvider
        where TResult : IEntity, new()

=======
﻿namespace ORBIT9000.Core.Abstractions.Providers.Data
{
    public interface IDataProvider { }
    public interface IDataProvider<TResult> : IDataProvider
        where TResult : IResult, new()
>>>>>>> 2e9d040 (Add Basic Plugin Channel Handling)
    {
        Task<IEnumerable<TResult>> GetData();
    }
}
