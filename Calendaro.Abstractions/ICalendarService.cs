namespace Calendaro.Abstractions
{
    /// <summary>
    /// Provides access to a calendar service.
    /// </summary>
    public interface ICalendarService
    {
        /// <summary>
        /// Retrieves information about all available calendars.
        /// </summary>
        /// <param name="cancellation">Cancellation token to stop calendars retrieval.</param>
        /// <returns>A <see cref="Task{TResult}"/> that asynchronously retrieves list of calendars.</returns>
        Task<IEnumerable<CalendarInfo>> ListCalendarsAsync(CancellationToken cancellation);

        /// <summary>
        /// Retrieves information about events in one calendar starting in the specified UTC time range.
        /// </summary>
        /// <param name="calendarId">Identifier of the calendar to retrieve events from.</param>
        /// <param name="windowStartTimeUtc">Lower bound (exclusive) for an event's end UTC time to filter by.</param>
        /// <param name="windowEndTimeUtc">Upper bound (exclusive) for an event's start UTC time to filter by.</param>
        /// <param name="cancellation">Cancellation token to stop events retrieval.</param>
        /// <returns>A <see cref="Task{TResult}"/> that asynchronously retrieves calendar events.</returns>
        Task<IEnumerable<CalendarEventInfo>> ListCalendarEventsAsync(
            string calendarId,
            DateTime windowStartTimeUtc,
            DateTime windowEndTimeUtc,
            CancellationToken cancellation);
    }
}
