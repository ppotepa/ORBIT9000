<<<<<<< HEAD
﻿using Terminal.Gui.CustomViews.Misc;
using static EngineTerminal.Managers.UIManager;
=======
﻿using EngineTerminal.Bindings;
using ORBIT9000.Core.Models.Pipe;
>>>>>>> 5ae5b98 (Add Inversion of Control)

namespace EngineTerminal.Contracts
{
    public interface IUIManager
    {
<<<<<<< HEAD
        Dictionary<string, ValueBinding> GridBindings { get; }

        void Initialize(object data);

        void Run();

        void UpdateUIFromData(object? sender, IReadOnlyList<BindingAction> updates);

        void UpdateStatusMessage(string? message, string? additionalInfo = null);
        void UpdateCurrentMethod(string? message);
=======
        Dictionary<string, ValueBinding> Bindings { get; }

        void Initialize(ExampleData initialData);

        void Run();

        void UpdateUIFromData(IReadOnlyList<Action<Dictionary<string, ValueBinding>>> updates);

        void UpdateStatusMessage(string message);
>>>>>>> 5ae5b98 (Add Inversion of Control)
    }
}