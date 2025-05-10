using EngineTerminal.Contracts;
using EngineTerminal.Proxies;
using ORBIT9000.Core.Models.Pipe.ORBIT9000.Core.Models.Pipe;
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
            if (EqualityComparer<TData?>.Default.Equals(newData, default) || this.Data == null)
                return [];

            if (newData is ExampleData typedNewData)
            {
                this._changeTracker.UpdateData(typedNewData);
            }

            IReadOnlyList<PropertyChangeRecord> changes = this._changeTracker.GetChanges();
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
            this._changeTracker.Initialize(initialData);
            this.Data = this._changeTracker.ProxyData;
        }

        private static ExampleData CreateInitialData()
            => RandomDataFiller.FillWithRandomData<ExampleData>();

        #endregion Methods
    }
}
