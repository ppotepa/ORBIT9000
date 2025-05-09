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

        private List<PropertyChangeRecord>? _changes;
        private TTargetType? _target;

        #endregion Fields

        #region Methods

        public PropertyChangeProxy<TTargetType> SetTarget(TTargetType target, List<PropertyChangeRecord> changes)
        {
            _target = target;
            _changes = changes;
            return this;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod == null || _target == null)
                throw new InvalidOperationException("Proxy not properly initialized");

            if (args == null || args.Length == 0)
                throw new ArgumentException("Arguments cannot be null or empty", nameof(args));

            string methodName = targetMethod.Name;

            try
            {
                if (targetMethod.IsSpecialName && methodName.StartsWith("set_"))
                {
                    string propertyName = methodName.Substring(4);
                    var propertyInfo = typeof(TTargetType).GetProperty(propertyName);

                    if (propertyInfo != null)
                    {
                        object? oldValue = propertyInfo.GetValue(_target);
                        object? result = targetMethod.Invoke(_target, args);
                        object? newValue = args[0];

                        if (!Equals(oldValue, newValue))
                        {
                            _changes?.Add(new PropertyChangeRecord
                            {
                                PropertyPath = propertyName,
                                OldValue = oldValue,
                                NewValue = newValue
                            });
                        }

                        return result!;
                    }
                }

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
