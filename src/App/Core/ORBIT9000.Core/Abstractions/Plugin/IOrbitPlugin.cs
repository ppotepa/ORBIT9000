namespace ORBIT9000.Core.Abstractions.Plugin
{
    public interface IOrbitPlugin
    {        
        Task OnLoad();
        Task OnUnload();
    }
}
