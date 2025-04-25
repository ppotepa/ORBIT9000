using System;
using Microsoft.Extensions.DependencyInjection;

namespace ORBIT9000.Core.Abstractions.Providers
{
    public class CompositeServiceProvider : IServiceProvider, IServiceScopeFactory, IServiceScope
    {
        private readonly IServiceProvider _primary, _secondary;
        private readonly IServiceProvider _primaryScope, _secondaryScope;
        private bool _disposed;

        public IServiceProvider ServiceProvider => throw new NotImplementedException();

        public CompositeServiceProvider(IServiceProvider primary, IServiceProvider secondary)
        {
            _primary = primary ?? throw new ArgumentNullException(nameof(primary));
            _secondary = secondary ?? throw new ArgumentNullException(nameof(secondary));

            _primaryScope = primary;
            _secondaryScope = secondary;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public object GetService(Type serviceType)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(CompositeServiceProvider));

            var instance = _primaryScope.GetService(serviceType);
            if (instance != null) return instance;

            instance = _secondaryScope.GetService(serviceType);
            if (instance != null) return instance;

            return ActivatorUtilities.CreateInstance(this, serviceType);
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(IServiceScopeFactory))
                return this;

            return GetService(serviceType);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_primaryScope is IDisposable pd) pd.Dispose();
                if (_secondaryScope is IDisposable sd) sd.Dispose();
            }

            _disposed = true;
        }

        public IServiceScope CreateScope()
        {
            throw new NotImplementedException();
        }
    }
}
