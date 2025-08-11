<<<<<<< HEAD
﻿using ORBIT9000.Core.Models.Pipe;
=======
﻿using ORBIT9000.Core.Models.Pipe.ORBIT9000.Core.Models.Pipe;
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
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

<<<<<<< HEAD
        private readonly List<PropertyChangeRecord> _changes = [];
        private readonly Dictionary<Type, PropertyInfo[]> _propertyCache = [];
        private TTargetType? _originalData;
        private TTargetType? _proxyData;
=======
        private TTargetType _originalData;
        private TTargetType _proxyData;

        private readonly List<PropertyChangeRecord> _changes = new();
        private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new();

>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
        #endregion Fields

        #region Properties

<<<<<<< HEAD
        public TTargetType ProxyData
        {
            get
            {
                if (_proxyData == null)
                {
                    throw new InvalidOperationException("Proxy data not initialized.");
                }

                return _proxyData;
            }
        }
=======
        public TTargetType ProxyData => _proxyData;
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)

        #endregion Properties

        #region Methods

<<<<<<< HEAD
        public IReadOnlyList<PropertyChangeRecord> GetChanges() => _changes;

=======
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
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
<<<<<<< HEAD
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
                return proxy.SetTarget(target, _changes) as TTargetType ?? throw new InvalidOperationException("Failed to create proxy.");
            }
            else
            {
                return target;
            }
        }

        private PropertyInfo[] GetCachedProperties(Type type)
        {
            if (!_propertyCache.TryGetValue(type, out PropertyInfo[]? properties))
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                _propertyCache[type] = properties;
            }
            return properties;
        }

        private void UpdateProperties<TProperty>(
                                    TProperty source,
=======

        public IReadOnlyList<PropertyChangeRecord> GetChanges() => _changes;

        private void UpdateProperties<TProperty>(
            TProperty source,
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
            TProperty target,
            string parentPath,
            List<PropertyChangeRecord> changes) where TProperty : class
        {
            if (source == null || target == null)
                return;

<<<<<<< HEAD
            PropertyInfo[] properties = GetCachedProperties(source.GetType());

            foreach (PropertyInfo property in properties)
=======
            var properties = GetCachedProperties(source.GetType());

            foreach (var property in properties)
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
            {
                if (!property.CanRead || !property.CanWrite)
                    continue;

<<<<<<< HEAD
                object? sourceValue = property.GetValue(source);
                object? targetValue = property.GetValue(target);
=======
                var sourceValue = property.GetValue(source);
                var targetValue = property.GetValue(target);
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)

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
<<<<<<< HEAD
=======

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

>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
        #endregion Methods
    }
}
