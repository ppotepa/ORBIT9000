using EngineTerminal.Bindings;
using ORBIT9000.Core.Models.Pipe;

namespace EngineTerminal.Contracts
{
    public interface IUIManager
    {
        Dictionary<string, ValueBinding> Bindings { get; }

        void Initialize(ExampleData initialData);

        void Run();

        void UpdateUIFromData(IReadOnlyList<Action<Dictionary<string, ValueBinding>>> updates);

        void UpdateStatusMessage(string message);
    }
}