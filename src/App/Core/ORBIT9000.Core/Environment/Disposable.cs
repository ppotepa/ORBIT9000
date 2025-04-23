namespace ORBIT9000.Core.Environment
{
    public abstract class Disposable : IDisposable
    {
        protected bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                DisposeManagedObjects();
            }

            DisposeUnmanagedObjects();
            disposed = true;
        }

        /// <summary>
        /// Free any managed objects
        /// </summary>
        protected virtual void DisposeManagedObjects() { }

        /// <summary>
        /// Free any unmanaged objects
        /// </summary>
        protected virtual void DisposeUnmanagedObjects() { }

        ~Disposable()
        {
            Dispose(false);
        }
    }
}
