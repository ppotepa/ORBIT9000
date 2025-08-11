<<<<<<< HEAD
<<<<<<< HEAD
﻿using Terminal.Gui.CustomViews.Misc;
using static EngineTerminal.Managers.UIManager;
=======
﻿using EngineTerminal.Bindings;
using EngineTerminal.Managers;
<<<<<<< HEAD
using ORBIT9000.Core.Models.Pipe;
>>>>>>> 5ae5b98 (Add Inversion of Control)
=======
>>>>>>> d246613 (Remove Tight Coupling Between Data Manager and Target Data Type)
=======
﻿using Terminal.Gui.CustomViews.Misc;
using static EngineTerminal.Managers.UIManager;
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)

namespace EngineTerminal.Contracts
{
    public interface IUIManager
    {
<<<<<<< HEAD
<<<<<<< HEAD
        Dictionary<string, ValueBinding> GridBindings { get; }

        void Initialize(object data);

        void Run();

        void UpdateUIFromData(object? sender, IReadOnlyList<BindingAction> updates);

        void UpdateStatusMessage(string? message, string? additionalInfo = null);
        void UpdateCurrentMethod(string? message);
=======
        Dictionary<string, ValueBinding> Bindings { get; }
=======
        Dictionary<string, ValueBinding> GridBindings { get; }
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)

        void Initialize(object data);

        void Run();

        void UpdateUIFromData(object sender, IReadOnlyList<BindingAction> updates);

<<<<<<< HEAD
        void UpdateStatusMessage(string message);
>>>>>>> 5ae5b98 (Add Inversion of Control)
=======
        void UpdateStatusMessage(string message, string additionalInfo = null);
        void UpdateCurrentMethod(string message);
>>>>>>> d246613 (Remove Tight Coupling Between Data Manager and Target Data Type)
    }
}