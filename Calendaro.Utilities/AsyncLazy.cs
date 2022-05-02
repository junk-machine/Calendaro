using System.Runtime.CompilerServices;

namespace Calendaro.Utilities
{
    /// <summary>
    /// Provides support for asynchronous lazy initialization.
    /// </summary>
    /// <typeparam name="T">Type of the object that is being lazily initialized.</typeparam>
    public sealed class AsyncLazy<T> : IDisposable
    {
        /// <summary>
        /// Delegate that initializes the object on first access.
        /// </summary>
        private readonly Func<CancellationToken, Task<T>> initializationDelegate;

        /// <summary>
        /// Asynchronous task that initializes the object.
        /// </summary>
        /// <remarks>
        /// We store and always return original initialization task. This way we don't need to manage our own semaphore,
        /// but this requires <see cref="IDisposable"/> implementation, which is likely not a bad thing, as we can also
        /// attempt to dispose wrapped object.
        /// </remarks>
        private Task<T>? initializationTask;

        /// <summary>
        /// Object used to synchronize concurrent initialization.
        /// </summary>
        private readonly object initializationSyncObject = new();

        /// <summary>
        /// Gets the value indicating whether object was already initialized.
        /// </summary>
        public bool IsInitialized => initializationTask is not null;

        /// <summary>
        /// Initialiazes a new instance of the <see cref="AsyncLazy"/> class
        /// with the provided initialization delegate.
        /// </summary>
        /// <param name="initializationDelegate">Delegate that initializes the object on first access.</param>
        public AsyncLazy(Func<CancellationToken, Task<T>> initializationDelegate)
        {
            this.initializationDelegate =
                initializationDelegate ?? throw new ArgumentNullException(nameof(initializationDelegate));
        }

        /// <summary>
        /// Lazily initializes the value, if it was not initialized yet.
        /// </summary>
        /// <param name="cancellation">Cancellation token to cancel initialization.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents an asynchronous initialization operation.</returns>
        public Task<T> GetValue(CancellationToken cancellation)
        {
            if (initializationTask == null)
            {
                lock (initializationSyncObject)
                {
                    if (initializationTask == null)
                    {
                        initializationTask = initializationDelegate(cancellation);
                    }
                }
            }

            return initializationTask;
        }

        /// <summary>
        /// Sets the value without invoking the initialization delegate.
        /// </summary>
        /// <param name="value">New value to set.</param>
        public void SetValue(T value)
        {
            lock (initializationSyncObject)
            {
                initializationTask = Task.FromResult(value);
            }
        }

        /// <summary>
        /// Disposes underlying object, if it was initialized.
        /// </summary>
        public void Dispose()
        {
            if (initializationTask != null)
            {
                if (initializationTask.IsCompletedSuccessfully
                    && initializationTask.Result is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                initializationTask.Dispose();
                initializationTask = null;
            }
        }
    }
}