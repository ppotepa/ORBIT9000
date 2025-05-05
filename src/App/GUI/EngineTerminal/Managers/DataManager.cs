using EngineTerminal.Contracts;
using EngineTerminal.Proxies;
using ORBIT9000.Core.Models.Pipe;
using ORBIT9000.Core.Models.Pipe.ORBIT9000.Core.Models.Pipe;
using TempTools;
using Terminal.Gui.CustomViews.Misc;
using static EngineTerminal.Managers.UIManager;

namespace EngineTerminal.Managers
{
    public class DataManager : IDataManager
    {
        #region Properties

        public object Data { get; private set; }
        private readonly ChangeTracker<IPipeData> _changeTracker = new();

        #endregion Properties

        #region Methods

        public IReadOnlyList<BindingAction> GetUpdates<TData>(TData newData, Dictionary<string, ValueBinding> bindings)
        {
            if (newData == null || Data == null)
                return new List<BindingAction>();

            if (newData is ExampleData typedNewData)
            {
                _changeTracker.UpdateData(typedNewData);
            }

            var changes = _changeTracker.GetChanges();
            var actions = new List<BindingAction>();

            foreach (var change in changes)
            {
                if (bindings.TryGetValue(change.PropertyPath, out var binding))
                {
                    var value = change.NewValue;
                    actions.Add(b => b[change.PropertyPath].Value = value);
                }
            }

            return actions;
        }

        public void Initialize()
        {
            var initialData = CreateInitialData();
            _changeTracker.Initialize(initialData);
            Data = _changeTracker.ProxyData;
        }

        private ExampleData CreateInitialData()
            => RandomDataFiller.FillWithRandomData<ExampleData>();

        #endregion Methods
    }
}
