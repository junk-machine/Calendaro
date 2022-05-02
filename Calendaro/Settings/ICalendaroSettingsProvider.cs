namespace Calendaro.Settings
{
    /// <summary>
    /// Maintains application settings.
    /// </summary>
    internal interface ICalendaroSettingsProvider
    {
        /// <summary>
        /// Retrieves application settings.
        /// </summary>
        /// <param name="cancellation">Cancellation token to stop the loading operation.</param>
        /// <returns>A <see cref="Task{TResult}"/>, which when completed provides application settings.</returns>
        Task<CalendaroSettings> LoadSettingsAsync(CancellationToken cancellation);

        /// <summary>
        /// Persists application settings.
        /// </summary>
        /// <param name="settings">New application settings to persist.</param>
        /// <param name="cancellation">Cancellation token to stop the save operation.</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous save operation.</returns>
        Task SaveSettingsAsync(CalendaroSettings settings, CancellationToken cancellation);
    }
}
