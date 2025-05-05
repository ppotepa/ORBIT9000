using EngineTerminal.Bindings;
using EngineTerminal.Managers;

namespace EngineTerminal.Contracts
{
    public interface IDataManager 
    {
        object Data { get; }

        IReadOnlyList<BindingAction> GetUpdates<TData>(TData newData, Dictionary<string, ValueBinding> bindings);

        void Initialize();
    }
}