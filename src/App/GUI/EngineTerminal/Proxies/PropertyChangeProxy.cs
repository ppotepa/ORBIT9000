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
<<<<<<< HEAD
        private List<PropertyChangeRecord>? _changes;
        private TTargetType? _target;
=======
        private List<PropertyChangeRecord> _changes;
        private TTargetType _target;
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
=======
        private List<PropertyChangeRecord>? _changes;
        private TTargetType? _target;
>>>>>>> 86e317a (Refactor interfaces and improve null safety)

        #endregion Fields

        #region Methods

        public PropertyChangeProxy<TTargetType> SetTarget(TTargetType target, List<PropertyChangeRecord> changes)
        {
            _target = target;
            _changes = changes;
            return this;
        }

<<<<<<< HEAD
<<<<<<< HEAD
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
=======
        protected override object Invoke(MethodInfo targetMethod, object[] args)
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
=======
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
>>>>>>> 86e317a (Refactor interfaces and improve null safety)
        {
            if (targetMethod == null || _target == null)
                throw new InvalidOperationException("Proxy not properly initialized");

<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
            if (args == null)
                throw new ArgumentException("Arguments cannot be null or empty", nameof(args));

=======
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
=======
            if (args == null || args.Length == 0)
=======
            if (args == null)
>>>>>>> 7978821 (Enhance scheduling and error handling in engine components)
                throw new ArgumentException("Arguments cannot be null or empty", nameof(args));

>>>>>>> 86e317a (Refactor interfaces and improve null safety)
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
                        object? oldValue = propertyInfo.GetValue(_target);
                        object? result = targetMethod.Invoke(_target, args);
                        object? newValue = args[0];

                        if (!Equals(oldValue, newValue))
                        {
<<<<<<< HEAD
                            _changes.Add(new PropertyChangeRecord
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
=======
                            _changes?.Add(new PropertyChangeRecord
>>>>>>> 86e317a (Refactor interfaces and improve null safety)
                            {
                                PropertyPath = propertyName,
                                OldValue = oldValue,
                                NewValue = newValue
                            });
                        }

<<<<<<< HEAD
<<<<<<< HEAD
                        return result!;
                    }
                }

=======
                        return result;
                    }
                }
>>>>>>> 13f95f8 (Add Dynamic Proxy instead of Object Traversal)
=======
                        return result!;
                    }
                }

>>>>>>> 86e317a (Refactor interfaces and improve null safety)
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
