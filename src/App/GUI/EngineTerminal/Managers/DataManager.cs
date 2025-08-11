<<<<<<< HEAD
﻿using EngineTerminal.Contracts;
using EngineTerminal.Proxies;
using ORBIT9000.Core.Models.Pipe;
using ORBIT9000.Core.TempTools;
using Terminal.Gui.CustomViews.Misc;
using static EngineTerminal.Managers.UIManager;

namespace EngineTerminal.Managers
{
    public class DataManager : IDataManager
    {
        #region Properties

        public object? Data { get; private set; }
        private readonly ChangeTracker<IPipeData> _changeTracker = new();

        #endregion Properties

        #region Methods

        public IReadOnlyList<BindingAction> GetUpdates<TData>(TData newData, Dictionary<string, ValueBinding> bindings)
        {
            if (EqualityComparer<TData?>.Default.Equals(newData, default) || Data == null)
                return [];

            if (newData is ExampleData typedNewData)
            {
                _changeTracker.UpdateData(typedNewData);
            }

            IReadOnlyList<PropertyChangeRecord> changes = _changeTracker.GetChanges();
            List<BindingAction> actions = [];

            foreach (PropertyChangeRecord change in changes)
            {
                if (bindings.TryGetValue(change.PropertyPath, out ValueBinding? _))
                {
                    object? value = change.NewValue;
                    actions.Add(b => b[change.PropertyPath].Value = value);
                }
            }

            return actions;
        }

        public void Initialize()
        {
            ExampleData initialData = CreateInitialData();
            _changeTracker.Initialize(initialData);
            Data = _changeTracker.ProxyData;
        }

        private static ExampleData CreateInitialData()
            => RandomDataFiller.FillWithRandomData<ExampleData>();

        #endregion Methods
    }
}
=======
﻿using EngineTerminal.Bindings;
using EngineTerminal.Contracts;
using ORBIT9000.Core.Models.Pipe;
using System.Reflection;

namespace EngineTerminal.Managers
{
    public class DataManager : IDataManager
    {
        public ExampleData ExampleData { get; private set; }

        public IReadOnlyList<BindingAction> GetUpdates(ExampleData newData, Dictionary<string, ValueBinding> bindings)
        {
            List<BindingAction> list = new List<BindingAction>();

            if (newData.Frame1 != null && newData.Frame1 != ExampleData.Frame1)
                list.AddRange(GetUpdateActions(newData.Frame1, ExampleData.Frame1, nameof(newData.Frame1)));

            if (newData.Frame2 != null && newData.Frame2 != ExampleData.Frame2)
                list.AddRange(GetUpdateActions(newData.Frame2, ExampleData.Frame2, nameof(newData.Frame2)));

            return list;
        }

        public void Initialize()
        {
            ExampleData = CreateInitialData();
        }

        private ExampleData CreateInitialData()
            => new ExampleData
            {
                Frame1 = new SettingsData { Setting1 = 1, Setting2 = "Text2" },
                Frame2 = new EngineData { Setting1 = 100, Setting2 = 200, IsValid = false }
            };

        private IEnumerable<BindingAction> GetUpdateActions(object source, object target, string parentPropertyName)
        {
            if (source == null || target == null)
                yield break;

            foreach (var property in source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!property.CanRead || !property.CanWrite) continue;

                var sourceValue = property.GetValue(source);
                var targetValue = property.GetValue(target);

                if (!Equals(sourceValue, targetValue))
                {
                    yield return bindings =>
                    {
                        property.SetValue(target, sourceValue);
                        bindings[$"{parentPropertyName}.{property.Name}"].Value = sourceValue;
                    };
                }
            }
        }
    }
<<<<<<< HEAD
}
>>>>>>> 80f2a0e (Split Responsibilities To Managers)
=======
}
>>>>>>> 5ae5b98 (Add Inversion of Control)
