using EngineTerminal.Bindings;
using EngineTerminal.Contracts;
using ORBIT9000.Core.Models.Pipe;
using System.Reflection;
using TempTools;

namespace EngineTerminal.Managers
{
    public class DataManager : IDataManager
    {
        #region Properties

        public ExampleData ExampleData { get; private set; }

        #endregion Properties

        #region Methods

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
            => RandomDataFiller.FillWithRandomData<ExampleData>();  

        private static readonly Dictionary<Type, PropertyInfo[]> PropertyCache = new();

        private IEnumerable<BindingAction> GetUpdateActions(object source, object target, string parentPropertyName)
        {
            if (source == null || target == null)
                yield break;

            var sourceType = source.GetType();

            if (!PropertyCache.TryGetValue(sourceType, out var properties))
            {
                properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                PropertyCache[sourceType] = properties;
            }

            foreach (var property in properties)
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

        #endregion Methods
    }
}