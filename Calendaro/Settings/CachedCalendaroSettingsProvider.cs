namespace Calendaro.Settings
{
    /// <summary>
    /// Settings provider wrapper that caches active settings.
    /// </summary>
    internal sealed class CachedCalendaroSettingsProvider : ICalendaroSettingsProvider
    {
        /// <summary>
        /// Underlying settings provider used to save and restore settings.
        /// </summary>
        private readonly ICalendaroSettingsProvider underlyingProvider;

        /// <summary>
        /// Lazily initialzed settings loading task.
        /// </summary>
        private Task<CalendaroSettings>? settingsRetrievalTask;

        /// <summary>
        /// An object used to synchronize <see cref="settingsRetrievalTask"/> field initialization.
        /// </summary>
        private readonly object retrievalTaskSyncObj = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCalendaroSettingsProvider"/> class
        /// with the provided underlying settings provider.
        /// </summary>
        /// <param name="underlyingProvider">Underlying settings provider used to save and restore settings.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public CachedCalendaroSettingsProvider(ICalendaroSettingsProvider underlyingProvider)
        {
            this.underlyingProvider =
                underlyingProvider ?? throw new ArgumentNullException(nameof(underlyingProvider));
        }

        /// <summary>
        /// Requests settings from the underlying settings provider, if there is no cached instance.
        /// Otherwise cached instance is returned.
        /// </summary>
        /// <param name="cancellation">Cancellation token to stop the loading process.</param>
        /// <returns>A <see cref="Task{TResult}"/>, which when completed provides application settings.</returns>
        public Task<CalendaroSettings> LoadSettingsAsync(CancellationToken cancellation)
        {
            if (settingsRetrievalTask == null)
            {
                lock (retrievalTaskSyncObj)
                {
                    if (settingsRetrievalTask == null)
                    {
                        settingsRetrievalTask = underlyingProvider.LoadSettingsAsync(cancellation);
                    }
                }
            }

            return settingsRetrievalTask;
        }

        /// <summary>
        /// Saves settings using underlying settings provider and updates the cached instance.
        /// </summary>
        /// <param name="settings">New application settings.</param>
        /// <param name="cancellation">Cancellation token to stop the saving process.</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous save operation.</returns>
        public async Task SaveSettingsAsync(CalendaroSettings settings, CancellationToken cancellation)
        {
            await underlyingProvider.SaveSettingsAsync(settings, cancellation);
            settingsRetrievalTask = Task.FromResult(settings);
        }
    }
}
