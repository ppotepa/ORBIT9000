namespace ORBIT9000.Core.Authentication
{

    public interface IAuthenticate
    {
        public abstract bool AllowAnonymous { get; }
        public abstract bool IsAuthenticated { get; }
        public abstract IAuthResult Authenticate();
    }

    public interface IAuthResult
    {
    }
}