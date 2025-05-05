using Terminal.Gui.CustomViews.Misc;
using static EngineTerminal.Managers.UIManager;

namespace EngineTerminal.Contracts
{
    public interface IDataManager 
    {
        object Data { get; }

        IReadOnlyList<BindingAction> GetUpdates<TData>(TData newData, Dictionary<string, ValueBinding> bindings);

        void Initialize();
    }
}