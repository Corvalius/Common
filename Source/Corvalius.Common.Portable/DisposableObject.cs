using System;
using System.Runtime.InteropServices;

namespace Corvalius.Common
{
    public class DisposableObject : IDisposable
    {
        // Fields
        private EventHandler disposing = (sender, obj) => { };

        // Events
        public event EventHandler Disposing
        {
            add
            {
                this.ThrowIfDisposed();
                this.disposing += value;
            }
            remove
            {
                this.ThrowIfDisposed();
                this.disposing -= value;
            }
        }

        // Methods
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                if (this.disposing != null)
                    this.disposing(this, new EventArgs());

                if (disposing)
                {
                    this.DisposeManagedResources();
                }
                this.DisposeNativeResources();
            }
            this.IsDisposed = true;
        }

        protected virtual void DisposeManagedResources()
        {
        }

        protected virtual void DisposeNativeResources()
        {
        }

        ~DisposableObject()
        {
            this.Dispose(false);
        }

        protected void ThrowIfDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        // Properties
        public bool IsDisposed { get; private set; }
    }
}