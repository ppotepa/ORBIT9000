using ORBIT9000.Core.Models.Pipe.ORBIT9000.Core.Models.Pipe;
using System.Reflection;

namespace EngineTerminal.Proxies
{
    /// <summary>
    /// Tracks changes to an object of type TTargetType using a dynamic proxy
    /// </summary>
    /// <typeparam name="TTargetType">Type of object to track</typeparam>
    public class ChangeTracker<TTargetType> where TTargetType : class
    {
        #region Fields

        private TTargetType _originalData;
        private TTargetType _proxyData;

        private readonly List<PropertyChangeRecord> _changes = new();
        private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new();

        #endregion Fields

        #region Properties

        public TTargetType ProxyData => _proxyData;

        #endregion Properties

        #region Methods

        public void Initialize(TTargetType data)
        {
            _originalData = data;
            _proxyData = CreateProxy(data);
        }

        public void UpdateData<TSource>(TSource newData) where TSource : class
        {
            if (newData == null || _originalData == null)
                return;

            _changes.Clear();

            if (_originalData is IPipeData originalPipeData && newData is IPipeData newPipeData)
            {
                if (newPipeData.Frame1 != null && originalPipeData.Frame1 != null)
                {
                    UpdateProperties(
                        newPipeData.Frame1,
                        originalPipeData.Frame1,
                        "Frame1",
                        _changes);
                }

                if (newPipeData.Frame2 != null && originalPipeData.Frame2 != null)
                {
                    UpdateProperties(
                        newPipeData.Frame2,
                        originalPipeData.Frame2,
                        "Frame2",
                        _changes);
                }
            }
        }

        public IReadOnlyList<PropertyChangeRecord> GetChanges() => _changes;

        private void UpdateProperties<TProperty>(
            TProperty source,
            TProperty target,
            string parentPath,
            List<PropertyChangeRecord> changes) where TProperty : class
        {
            if (source == null || target == null)
                return;

            var properties = GetCachedProperties(source.GetType());

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                    continue;

                var sourceValue = property.GetValue(source);
                var targetValue = property.GetValue(target);

                string propertyPath = string.IsNullOrEmpty(parentPath)
                    ? property.Name
                    : $"{parentPath}.{property.Name}";

                if (!Equals(sourceValue, targetValue))
                {
                    changes.Add(new PropertyChangeRecord
                    {
                        PropertyPath = propertyPath,
                        OldValue = targetValue,
                        NewValue = sourceValue
                    });

                    property.SetValue(target, sourceValue);
                }

                if (IsComplexType(property.PropertyType) &&
                    sourceValue != null && targetValue != null)
                {
                    UpdateProperties(sourceValue, targetValue, propertyPath, changes);
                }
            }
        }

        private TTargetType CreateProxy(TTargetType target)
        {
            if (DispatchProxy.Create<TTargetType, PropertyChangeProxy<TTargetType>>() is PropertyChangeProxy<TTargetType> proxy)
            {
                return proxy.SetTarget(target, _changes) as TTargetType;
            }
            else
            {
                return target;
            }
        }

        private bool IsComplexType(Type type)
        {
            return !type.IsPrimitive &&
                   type != typeof(string) &&
                   !type.IsEnum &&
                   !type.IsValueType;
        }

        private PropertyInfo[] GetCachedProperties(Type type)
        {
            if (!_propertyCache.TryGetValue(type, out var properties))
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                _propertyCache[type] = properties;
            }
            return properties;
        }

        #endregion Methods
    }
}
