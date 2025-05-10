namespace ORBIT9000.Core.Environment
{
    public abstract class Disposable : IDisposable
    {
        protected bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing)
            {
                this.DisposeManagedObjects();
            }

            this.DisposeUnmanagedObjects();
            this.disposed = true;
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
            this.Dispose(false);
        }
    }
}
