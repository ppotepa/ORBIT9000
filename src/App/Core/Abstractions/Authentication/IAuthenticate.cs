<<<<<<< HEAD
﻿namespace ORBIT9000.Abstractions.Authentication
{
    public interface IAuthenticate
    {
        abstract bool AllowAnonymous { get; }
        abstract bool IsAuthenticated { get; }
        abstract IAuthResult Authenticate();
=======
﻿namespace ORBIT9000.Core.Abstractions.Authentication
{
    public interface IAuthenticate
    {
        public abstract bool AllowAnonymous { get; }
        public abstract bool IsAuthenticated { get; }
        public abstract IAuthResult Authenticate();
>>>>>>> 2e9d040 (Add Basic Plugin Channel Handling)
    }
}