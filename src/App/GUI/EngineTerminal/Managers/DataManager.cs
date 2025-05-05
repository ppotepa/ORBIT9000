using EngineTerminal.Bindings;
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
}