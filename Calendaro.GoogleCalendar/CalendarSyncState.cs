using Calendaro.Abstractions;
using Calendaro.Utilities;

namespace Calendaro.GoogleCalendar
{
    /// <summary>
    /// Maintains synchronization state for a single calendar.
    /// </summary>
    /// <remarks>
    /// Instead of querying all events all the time, we rely on 'change feed' using sync token.
    /// This means we will request all events for a prolonged interval (week or month) once a day,
    /// but during the day we will only request changes. Changes will be merged into the cache
    /// maintained by this state object and all <see cref="GoogleCalendarService.ListCalendarEventsAsync(string, DateTime, DateTime, CancellationToken)"/>
    /// calls will be answered from this cache.
    /// </remarks>
    internal sealed class CalendarSyncState : IDisposable
    {
        /// <summary>
        /// Gets the identifier of the calendar to which this state belongs.
        /// </summary>
        public string CalendarId { get; }

        /// <summary>
        /// Gets the cache of synchronized events.
        /// </summary>
        public PooledSortedList<CalendarEventInfo> CachedEvents { get; }

        /// <summary>
        /// Gets or sets the number of days since `01-01-0001` when last full sync was performed.
        /// </summary>
        /// <remarks>
        /// We force full sync every day, so this helps us to identify when next full sync needs to be performed.
        /// </remarks>
        public long LastSyncDay { get; set; }

        /// <summary>
        /// Gets or sets the sync token for the Google Calendar list events request used to only retrieve new and changed events.
        /// </summary>
        public string? IncrementalSyncToken { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarSyncState"/> class
        /// with the provided calendar identifier.
        /// </summary>
        /// <param name="calendarId">Identifier of the calendar to which this state belongs.</param>
        public CalendarSyncState(string calendarId)
        {
            CalendarId = calendarId;

            // Store cached events in the start time order to speed up range lookup
            CachedEvents =
                new PooledSortedList<CalendarEventInfo>(
                    Comparer<CalendarEventInfo>.Create(
                        (x, y) => Comparer<DateTime?>.Default.Compare(x?.StartTimeUtc, y?.StartTimeUtc)));
        }

        /// <summary>
        /// Disposes the cached events list.
        /// </summary>
        public void Dispose()
        {
            CachedEvents.Dispose();
        }
    }
}
