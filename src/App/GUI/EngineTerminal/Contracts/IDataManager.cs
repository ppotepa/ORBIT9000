<<<<<<< HEAD
﻿using Terminal.Gui.CustomViews.Misc;
using static EngineTerminal.Managers.UIManager;
=======
﻿using EngineTerminal.Bindings;
using ORBIT9000.Core.Models.Pipe;
>>>>>>> 5ae5b98 (Add Inversion of Control)

namespace EngineTerminal.Contracts
{
    public interface IDataManager
    {
<<<<<<< HEAD
        object? Data { get; }

        IReadOnlyList<BindingAction> GetUpdates<TData>(TData newData, Dictionary<string, ValueBinding> bindings);
=======
        ExampleData ExampleData { get; }

        IReadOnlyList<Action<Dictionary<string, ValueBinding>>> GetUpdates(ExampleData newData, Dictionary<string, ValueBinding> bindings);
>>>>>>> 5ae5b98 (Add Inversion of Control)

        void Initialize();
    }
}