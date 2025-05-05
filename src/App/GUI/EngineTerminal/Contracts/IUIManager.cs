using EngineTerminal.Bindings;
using EngineTerminal.Managers;

namespace EngineTerminal.Contracts
{
    public interface IUIManager
    {
        Dictionary<string, ValueBinding> Bindings { get; }

        void Initialize(object data);

        void Run();

        void UpdateUIFromData(object sender, IReadOnlyList<BindingAction> updates);

        void UpdateStatusMessage(string message, string additionalInfo = null);
        void UpdateCurrentMethod(string message);
    }
}