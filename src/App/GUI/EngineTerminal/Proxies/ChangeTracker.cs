using ORBIT9000.Core.Models.Pipe;
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

        private readonly List<PropertyChangeRecord> _changes = [];
        private readonly Dictionary<Type, PropertyInfo[]> _propertyCache = [];
        private TTargetType? _originalData;
        private TTargetType? _proxyData;
        #endregion Fields

        #region Properties

        public TTargetType ProxyData
        {
            get
            {
                if (this._proxyData == null)
                {
                    throw new InvalidOperationException("Proxy data not initialized.");
                }

                return this._proxyData;
            }
        }

        #endregion Properties

        #region Methods

        public IReadOnlyList<PropertyChangeRecord> GetChanges() => this._changes;

        public void Initialize(TTargetType data)
        {
            this._originalData = data;
            this._proxyData = this.CreateProxy(data);
        }

        public void UpdateData<TSource>(TSource newData) where TSource : class
        {
            if (newData == null || this._originalData == null)
                return;

            this._changes.Clear();

            if (this._originalData is IPipeData originalPipeData && newData is IPipeData newPipeData)
            {
                if (newPipeData.Frame1 != null && originalPipeData.Frame1 != null)
                {
                    this.UpdateProperties(
                        newPipeData.Frame1,
                        originalPipeData.Frame1,
                        "Frame1",
                        this._changes);
                }

                if (newPipeData.Frame2 != null && originalPipeData.Frame2 != null)
                {
                    this.UpdateProperties(
                        newPipeData.Frame2,
                        originalPipeData.Frame2,
                        "Frame2",
                        this._changes);
                }
            }
        }
        private static bool IsComplexType(Type type)
        {
            return !type.IsPrimitive &&
                   type != typeof(string) &&
                   !type.IsEnum &&
                   !type.IsValueType;
        }

        private TTargetType CreateProxy(TTargetType target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target), "Target cannot be null.");
            }

            if (DispatchProxy.Create<TTargetType, PropertyChangeProxy<TTargetType>>() is PropertyChangeProxy<TTargetType> proxy)
            {
                return proxy.SetTarget(target, this._changes) as TTargetType ?? throw new InvalidOperationException("Failed to create proxy.");
            }
            else
            {
                return target;
            }
        }

        private PropertyInfo[] GetCachedProperties(Type type)
        {
            if (!this._propertyCache.TryGetValue(type, out PropertyInfo[]? properties))
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                this._propertyCache[type] = properties;
            }
            return properties;
        }

        private void UpdateProperties<TProperty>(
                                    TProperty source,
            TProperty target,
            string parentPath,
            List<PropertyChangeRecord> changes) where TProperty : class
        {
            if (source == null || target == null)
                return;

            PropertyInfo[] properties = this.GetCachedProperties(source.GetType());

            foreach (PropertyInfo property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                    continue;

                object? sourceValue = property.GetValue(source);
                object? targetValue = property.GetValue(target);

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
                    this.UpdateProperties(sourceValue, targetValue, propertyPath, changes);
                }
            }
        }
        #endregion Methods
    }
}
