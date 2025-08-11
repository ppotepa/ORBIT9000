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
    public interface IDataManager 
    {
<<<<<<< HEAD
<<<<<<< HEAD
        object? Data { get; }

        IReadOnlyList<BindingAction> GetUpdates<TData>(TData newData, Dictionary<string, ValueBinding> bindings);
=======
        ExampleData ExampleData { get; }

<<<<<<< HEAD
        IReadOnlyList<Action<Dictionary<string, ValueBinding>>> GetUpdates(ExampleData newData, Dictionary<string, ValueBinding> bindings);
>>>>>>> 5ae5b98 (Add Inversion of Control)
=======
        IReadOnlyList<BindingAction> GetUpdates(ExampleData newData, Dictionary<string, ValueBinding> bindings);
>>>>>>> 18f5855 (Replace Dictionary of Actions with more clean BindingAction Type)
=======
        object Data { get; }

        IReadOnlyList<BindingAction> GetUpdates<TData>(TData newData, Dictionary<string, ValueBinding> bindings);
>>>>>>> d246613 (Remove Tight Coupling Between Data Manager and Target Data Type)

        void Initialize();
    }
}