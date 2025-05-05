using Terminal.Gui.CustomViews.Misc;
using static EngineTerminal.Managers.UIManager;

namespace EngineTerminal.Contracts
{
    public interface IUIManager
    {
        Dictionary<string, ValueBinding> GridBindings { get; }

        void Initialize(object data);

        void Run();

        void UpdateUIFromData(object sender, IReadOnlyList<BindingAction> updates);

        void UpdateStatusMessage(string message, string additionalInfo = null);
        void UpdateCurrentMethod(string message);
    }
}