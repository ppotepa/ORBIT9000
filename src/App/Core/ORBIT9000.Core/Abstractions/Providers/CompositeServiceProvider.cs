using Microsoft.Extensions.DependencyInjection;

namespace ORBIT9000.Core.Abstractions.Providers
{
    public class CompositeServiceProvider : IServiceProvider, IServiceScopeFactory, IServiceScope
    {
        private readonly IServiceProvider _primary, _secondary;
        private readonly IServiceProvider _primaryScope, _secondaryScope;
        private bool _disposed;

        public CompositeServiceProvider(IServiceProvider primary, IServiceProvider secondary)
        {
            _primary = primary;
            _secondary = secondary;

            _primaryScope = primary;
            _secondaryScope = secondary;
        }

        public IServiceProvider ServiceProvider => _primaryScope ?? _secondaryScope;

        public IServiceScope CreateScope()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(CompositeServiceProvider));

            var pFactory = _primary.GetService<IServiceScopeFactory>();
            var sFactory = _secondary.GetService<IServiceScopeFactory>();

            var pScope = pFactory?.CreateScope() ?? throw new InvalidOperationException("Primary has no scope factory");
            var sScope = sFactory?.CreateScope() ?? throw new InvalidOperationException("Secondary has no scope factory");

            return new CompositeServiceProvider(pScope.ServiceProvider, sScope.ServiceProvider);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

        public object GetService(Type serviceType)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(CompositeServiceProvider));

            var svc = _primaryScope.GetService(serviceType);
            if (svc != null) return svc;

            return _secondaryScope.GetService(serviceType)!;
        }

        object IServiceProvider.GetService(Type serviceType)
            => serviceType == typeof(IServiceScopeFactory)
               ? this
               : GetService(serviceType);
    }
}