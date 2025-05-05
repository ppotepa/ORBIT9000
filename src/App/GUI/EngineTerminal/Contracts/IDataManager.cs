using EngineTerminal.Bindings;
using EngineTerminal.Managers;
using ORBIT9000.Core.Models.Pipe;

namespace EngineTerminal.Contracts
{
    public interface IDataManager
    {
        ExampleData ExampleData { get; }

        IReadOnlyList<BindingAction> GetUpdates(ExampleData newData, Dictionary<string, ValueBinding> bindings);

        void Initialize();
    }
}