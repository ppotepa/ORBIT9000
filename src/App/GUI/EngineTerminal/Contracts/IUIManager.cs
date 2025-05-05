using EngineTerminal.Bindings;
using EngineTerminal.Managers;
using ORBIT9000.Core.Models.Pipe;

namespace EngineTerminal.Contracts
{
    public interface IUIManager
    {
        Dictionary<string, ValueBinding> Bindings { get; }

        void Initialize(ExampleData initialData);

        void Run();

        void UpdateUIFromData(object sender, IReadOnlyList<BindingAction> updates);

        void UpdateStatusMessage(string message);
    }
}