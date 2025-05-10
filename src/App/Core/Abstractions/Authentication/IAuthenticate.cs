namespace ORBIT9000.Core.Abstractions.Authentication
{
    public interface IAuthenticate
    {
        abstract bool AllowAnonymous { get; }
        abstract bool IsAuthenticated { get; }
        abstract IAuthResult Authenticate();
    }
}