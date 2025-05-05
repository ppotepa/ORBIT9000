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

        public object Data { get; private set; }
        private static readonly Dictionary<Type, PropertyInfo[]> PropertyCache = new();

        #endregion Properties

        #region Methods

        public IReadOnlyList<BindingAction> GetUpdates<TData>(TData newData, Dictionary<string, ValueBinding> bindings)
        {
            if (newData == null || Data == null)
                return new List<BindingAction>();

            return CompareAndGetUpdates(newData, Data, string.Empty).ToList();
        }

        private IEnumerable<BindingAction> CompareAndGetUpdates(object newData, object currentData, string parentPath)
        {
            if (newData == null || currentData == null)
                yield break;

            var properties = GetCachedProperties(newData.GetType());

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                    continue;

                var newValue = property.GetValue(newData);
                if (newValue == null)
                    continue;

                var currentValue = property.GetValue(currentData);
                if (Equals(newValue, currentValue))
                    continue;

                string propertyPath = BuildPropertyPath(parentPath, property.Name);

                if (IsComplexType(property.PropertyType, newValue, currentValue))
                {
                    foreach (var action in GetUpdateActions(newValue, currentValue, propertyPath))
                        yield return action;
                }
                else
                {
                    yield return CreatePropertyUpdateAction(property, currentData, newValue, propertyPath);
                }
            }
        }

        private bool IsComplexType(Type propertyType, object newValue, object currentValue)
        {
            return newValue != null &&
                   currentValue != null &&
                   !propertyType.IsPrimitive &&
                   propertyType != typeof(string) &&
                   !propertyType.IsEnum;
        }

        private string BuildPropertyPath(string parentPath, string propertyName)
        {
            return string.IsNullOrEmpty(parentPath) ? propertyName : $"{parentPath}.{propertyName}";
        }

        private BindingAction CreatePropertyUpdateAction(PropertyInfo property, object target, object newValue, string propertyPath)
        {
            return bindings =>
            {
                property.SetValue(target, newValue);
                if (bindings.ContainsKey(propertyPath))
                {
                    bindings[propertyPath].Value = newValue;
                }
            };
        }

        private PropertyInfo[] GetCachedProperties(Type type)
        {
            if (!PropertyCache.TryGetValue(type, out var properties))
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                PropertyCache[type] = properties;
            }
            return properties;
        }

        public void Initialize()
        {
            Data = CreateInitialData();
        }

        private ExampleData CreateInitialData()
            => RandomDataFiller.FillWithRandomData<ExampleData>();

        private IEnumerable<BindingAction> GetUpdateActions(object source, object target, string parentPropertyName)
        {
            if (source == null || target == null)
                yield break;

            var properties = GetCachedProperties(source.GetType());

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                    continue;

                var sourceValue = property.GetValue(source);
                var targetValue = property.GetValue(target);

                if (!Equals(sourceValue, targetValue))
                {
                    string propertyPath = $"{parentPropertyName}.{property.Name}";
                    yield return CreatePropertyUpdateAction(property, target, sourceValue, propertyPath);
                }
            }
        }

        #endregion Methods
    }
}
