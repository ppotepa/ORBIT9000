using System.Reflection;

namespace EngineTerminal.Proxies
{
    /// <summary>
    /// Dynamic proxy that intercepts property access for change tracking
    /// </summary>
    /// <typeparam name="TTargetType">Type to proxy</typeparam>
    public class PropertyChangeProxy<TTargetType> : DispatchProxy where TTargetType : class
    {
        #region Fields

<<<<<<< HEAD
        private List<PropertyChangeRecord>? _changes;
        private TTargetType? _target;
=======
        private List<PropertyChangeRecord> _changes;
        private TTargetType _target;
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)

        #endregion Fields

        #region Methods

        public PropertyChangeProxy<TTargetType> SetTarget(TTargetType target, List<PropertyChangeRecord> changes)
        {
            _target = target;
            _changes = changes;
            return this;
        }

<<<<<<< HEAD
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
=======
        protected override object Invoke(MethodInfo targetMethod, object[] args)
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
        {
            if (targetMethod == null || _target == null)
                throw new InvalidOperationException("Proxy not properly initialized");

<<<<<<< HEAD
            if (args == null)
                throw new ArgumentException("Arguments cannot be null or empty", nameof(args));

=======
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
            string methodName = targetMethod.Name;

            try
            {
                if (targetMethod.IsSpecialName && methodName.StartsWith("set_"))
                {
<<<<<<< HEAD
                    string propertyName = methodName[4..];
                    PropertyInfo? propertyInfo = typeof(TTargetType).GetProperty(propertyName);

                    if (propertyInfo != null)
                    {
                        object? oldValue = propertyInfo.GetValue(_target);
                        object? result = targetMethod.Invoke(_target, args);
                        object? newValue = args[0];

                        if (!Equals(oldValue, newValue))
                        {
                            _changes?.Add(new PropertyChangeRecord
=======
                    string propertyName = methodName.Substring(4);
                    var propertyInfo = typeof(TTargetType).GetProperty(propertyName);

                    if (propertyInfo != null)
                    {
                        object oldValue = propertyInfo.GetValue(_target);
                        object result = targetMethod.Invoke(_target, args);
                        object newValue = args[0];

                        if (!Equals(oldValue, newValue))
                        {
                            _changes.Add(new PropertyChangeRecord
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
                            {
                                PropertyPath = propertyName,
                                OldValue = oldValue,
                                NewValue = newValue
                            });
                        }

<<<<<<< HEAD
                        return result!;
                    }
                }

=======
                        return result;
                    }
                }
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
                return targetMethod.Invoke(_target, args);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        #endregion Methods
    }
}
