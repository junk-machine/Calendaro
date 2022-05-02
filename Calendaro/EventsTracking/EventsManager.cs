using Calendaro.CalendarServices;
using Calendaro.Settings;
using Calendaro.Utilities;

namespace Calendaro.EventsTracking
{
    /// <summary>
    /// Manages the state of synchronized events from multiple calendars.
    /// </summary>
    /// <remarks>
    /// This implementation is not thread-safe and should not be called from multiple threads in parallel.
    /// </remarks>
    internal sealed class EventsManager : IDisposable
    {
        /// <summary>
        /// Factory used to create clients for different calendar services.
        /// </summary>
        private readonly ICalendarServicesFactory calendarServicesFactory;

        /// <summary>
        /// All synchronized events within the configured time window for active calendars.
        /// </summary>
        private readonly PooledSortedList<TrackedEvent> allEvents = new(StartTimeEventComparer.Instance);

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsManager"/> class
        /// with the provided calendar services factory.
        /// </summary>
        /// <param name="calendarServicesFactory">Factory used to create clients for different calendar services.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public EventsManager(ICalendarServicesFactory calendarServicesFactory)
        {
            this.calendarServicesFactory =
                calendarServicesFactory ?? throw new ArgumentNullException(nameof(calendarServicesFactory));
        }

        /// <summary>
        /// Releases all resources associated with this instance.
        /// </summary>
        public void Dispose()
        {
            allEvents.Dispose();
        }

        /// <summary>
        /// Retrieves events that are currently active.
        /// Active events are those that user should be notified about, either starting soon
        /// or already ongoing.
        /// </summary>
        /// <returns>Collection of active events that should be shown to the user.</returns>
        public IReadOnlyList<TrackedEvent> GetActiveEvents()
        {
            var utcNow = DateTime.UtcNow;
            var activeEvents = new List<TrackedEvent>();

            for (var eventIndex = 0; eventIndex < allEvents.Count; ++eventIndex)
            {
                var trackedEvent = allEvents[eventIndex];

                // Keep ended events for 15 minutes, just in case there is a time skew and
                // calendar service will return them again
                if (utcNow > trackedEvent.Event.EndTimeUtc.AddMinutes(15))
                {
                    // Event has already ended - remove the event
                    allEvents.RemoveAt(eventIndex);
                    --eventIndex;
                }
                else if (utcNow >= trackedEvent.RemindAtUtc)
                {
                    // We are at or past reminder time, this is an active event that should be shown to the user
                    activeEvents.Add(trackedEvent);
                }
            }

            return activeEvents;
        }

        /// <summary>
        /// Synchronizes events for all configured calendars.
        /// </summary>
        /// <param name="activeAccountsConfiguration">List of calendar accounts to synchronize events for.</param>
        /// <param name="lookaheadInterval">Time interval for which to pre-load events.</param>
        /// <param name="cancellation">Cancellation token to stop the synchronization process.</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous synchronization process.</returns>
        /// <exception cref="AggregateException">Synchronization of one or more calendars failed.</exception>
        public async Task SynchronizeEventsAsync(
            IEnumerable<CalendarAccountConfiguration> activeAccountsConfiguration,
            TimeSpan lookaheadInterval,
            CancellationToken cancellation)
        {
            var windowStartTime = DateTime.UtcNow;
            var windowEndTime = DateTime.UtcNow + lookaheadInterval;

            using var remoteEvents =
                new PooledSortedList<TrackedEvent>(StartTimeEventComparer.Instance);
            List<Exception>? syncErrors = null;
            HashSet<string>? failedCalendarIds = null;

            foreach (var account in activeAccountsConfiguration)
            {
                var calendarServiceClient =
                    calendarServicesFactory.Create(account.CalendarServiceType, account.AccountId);

                foreach (var calendar in account.Calendars)
                {
                    try
                    {
                        foreach (var @event
                            in await calendarServiceClient
                                .ListCalendarEventsAsync(calendar.Id, windowStartTime, windowEndTime, cancellation))
                        {
                            remoteEvents.Add(new TrackedEvent(calendar, @event));
                        }
                    }
                    catch (Exception syncError)
                    {
                        // The show must go on! We still update the list of events for the calendars that we
                        // were able to synchronize, and throw any errors at the very end. That way we can
                        // display events for the successfully synchronized calendars, at least.
                        if (syncErrors is null || failedCalendarIds is null)
                        {
                            syncErrors = new List<Exception>();
                            failedCalendarIds = new HashSet<string>();
                        }

                        syncErrors.Add(new CalendarSyncException(account, calendar, syncError));
                        failedCalendarIds.Add(calendar.Id);
                    }
                }
            }

            MergeEvents(remoteEvents, failedCalendarIds);

            if (syncErrors is not null)
            {
                throw new AggregateException(syncErrors);
            }
        }

        /// <summary>
        /// Adds new events, updates existing ones and removes non-existing from the <see cref="allEvents"/>
        /// based on the provided list of events in the remote calendars.
        /// </summary>
        /// <param name="remoteEvents">List of events in remote calendars.</param>
        /// <param name="persistCalendarIds">
        /// Set of calendar identifiers whose events should be persisted, even if they are not present in remote events list.
        /// This allows us to gracefully handle transient synchronization errors by ensuring that we are not going to remove
        /// calendars that failed to sync from the local events list.
        /// </param>
        private void MergeEvents(PooledSortedList<TrackedEvent> remoteEvents, IReadOnlySet<string>? persistCalendarIds)
        {
            var localEventIndex = 0;
            var remoteEventIndex = 0;
            
            while (localEventIndex < allEvents.Count
                && remoteEventIndex < remoteEvents.Count)
            {
                var localEvent = allEvents[localEventIndex];
                var remoteEvent = remoteEvents[remoteEventIndex];

                if (StartTimeEventComparer.Instance.Compare(localEvent, remoteEvent) > 0)
                {
                    // New remote event - add to local list
                    allEvents.Add(remoteEvent);

                    // Since events are ordered using the same comparer, we know this event will be inserted
                    // before the current `localEvent`, so we can safely skip it on the next iteration
                    ++localEventIndex;
                    ++remoteEventIndex;
                }
                else if (StartTimeEventComparer.Instance.Compare(localEvent, remoteEvent) < 0)
                {
                    // Local event is not in the remote calendar any longer
                    if (persistCalendarIds?.Contains(localEvent.Calendar.Id) == true)
                    {
                        // Events from this calendar should be persisted regardless, so just skip over it
                        ++localEventIndex;
                    }
                    else
                    {
                        // Remove event from local list
                        allEvents.RemoveAt(localEventIndex);
                    }
                }
                else
                {
                    // Same event - check if it has changed and update event from the remote
                    if (localEvent.Event.Title != remoteEvent.Event.Title
                        || localEvent.Event.EventUri != remoteEvent.Event.EventUri
                        || localEvent.Event.ConferenceUri != remoteEvent.Event.ConferenceUri
                        || localEvent.Event.ReminderInterval != remoteEvent.Event.ReminderInterval)
                    {
                        // Our events are read-only, so that you cannot change properties like StartTime,
                        // which impact order of the events. We could make some properties writable, but
                        // I'd rather prefer to enforce read-only, as we don't know how events are going
                        // to be sorted tomorrow. Instead we sacrifice a little bit of performance and
                        // simply remove our event and add new one from remote.
                        // This will reset snoozing, which maybe a good or a bad thing, depending on what
                        // has changed. Regardless, this should not happen too often, so should be fine.
                        allEvents.RemoveAt(localEventIndex);
                        allEvents.Add(remoteEvent);
                    }

                    ++localEventIndex;
                    ++remoteEventIndex;
                }
            }

            // If there are local events that were not processed yet, it means we have bunch of trailing events
            // that were removed in remote calendars.
            // Remove them in the local list in reverse, as it eliminates array copy internally.
            for (var removalIndex = allEvents.Count - 1; removalIndex >= localEventIndex; --removalIndex)
            {
                allEvents.RemoveAt(removalIndex);
            }

            // All remote events that were not processed yet are considered new and should be added to local list
            for (; remoteEventIndex < remoteEvents.Count; ++remoteEventIndex)
            {
                allEvents.Add(remoteEvents[remoteEventIndex]);
            }
        }
    }
}
