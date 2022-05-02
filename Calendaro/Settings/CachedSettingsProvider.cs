namespace Calendaro.Settings
{
    /// <summary>
    /// Settings provider class that caches the settings coming from the underlying provider.
    /// </summary>
    internal sealed class CachedSettingsProvider : ICalendaroSettingsProvider
    {
        /// <summary>
        /// Underlying settings provider used to load and save the settings.
        /// </summary>
        private readonly ICalendaroSettingsProvider underlyingProvider;

        /// <summary>
        /// Synchronization object for the <see cref="loadingTask"/> field initialization.
        /// </summary>
        private readonly object loadingTaskSemaphore = new object();

        /// <summary>
        /// Lazily initialized task to load settings from the underlying provider.
        /// </summary>
        private Task<CalendaroSettings>? loadingTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedSettingsProvider"/> class
        /// with the provided underlying settings provider.
        /// </summary>
        /// <param name="underlyingProvider">Underlying settings provider used to load and save the settings.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public CachedSettingsProvider(ICalendaroSettingsProvider underlyingProvider)
        {
            this.underlyingProvider =
                underlyingProvider ?? throw new ArgumentNullException(nameof(underlyingProvider));
        }

        /// <inheritdoc/>
        /// <summary>
        /// Loads the settings from the underlying provider, if they were not loaded yet.
        /// Otherwise, cached instance will be returned.
        /// </summary>
        public Task<CalendaroSettings> LoadSettingsAsync(CancellationToken cancellation)
        {
            if (loadingTask == null)
            {
                lock (loadingTaskSemaphore)
                {
                    if (loadingTask == null)
                    {
                        loadingTask = underlyingProvider.LoadSettingsAsync(cancellation);
                    }
                }
            }

            return loadingTask;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Saves the settings to the underlying provider and updates cached instance.
        /// </summary>
        public async Task SaveSettingsAsync(CalendaroSettings settings, CancellationToken cancellation)
        {
            await underlyingProvider.SaveSettingsAsync(settings, cancellation);
            loadingTask = Task.FromResult(settings);
        }
    }
}
