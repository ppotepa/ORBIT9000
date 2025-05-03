using EngineTerminal.Bindings;
using ORBIT9000.Core.Models.Pipe;

namespace EngineTerminal.Contracts
{
    public interface IDataManager
    {
        ExampleData ExampleData { get; }

        IReadOnlyList<Action<Dictionary<string, ValueBinding>>> GetUpdates(ExampleData newData, Dictionary<string, ValueBinding> bindings);

        void Initialize();
    }
}