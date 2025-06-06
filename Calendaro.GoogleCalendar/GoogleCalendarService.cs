using Calendaro.Abstractions;
using Calendaro.GoogleCalendar.Properties;
using Calendaro.Utilities;
using Google;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace Calendaro.GoogleCalendar
{
    /// <summary>
    /// Calendar service that retreives data for the single Google Calendar account.
    /// </summary>
    public sealed class GoogleCalendarService : ICalendarService, IDisposable
    {
        /// <summary>
        /// Reminder method that represents UI notification in Google Calendar API.
        /// </summary>
        private const string NotificationReminderMethod = "popup";

        /// <summary>
        /// Type of the conference entry point that represents a video call.
        /// </summary>
        private const string VideoConferenceEntryPointType = "video";

        /// <summary>
        /// OAuth error type that represents an expired or otherwise stale token.
        /// </summary>
        private const string OAuthInvalidGrantError = "invalid_grant";

        /// <summary>
        /// Regular expression to determine whether URI is a Zoom video conference link.
        /// </summary>
        private static readonly Regex ZoomLinkRegex =
            new(@"https?://[^\.]+\.zoom\.us/j/[^\s]*", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Gets or sets the Google API authenticator used to obtain access token.
        /// </summary>
        private readonly OAuth2Authenticator authenticator;

        /// <summary>
        /// Client for the Google Calendar APIs.
        /// </summary>
        private AsyncLazy<CalendarService> calendarServiceClient;

        /// <summary>
        /// States of all actively synchronized calendars.
        /// </summary>
        /// <remarks>
        /// See remarks section for <see cref="CalendarSyncState"/> for more details.
        /// </remarks>
        private readonly PooledSortedList<CalendarSyncState> calendarSyncStates;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleCalendarService"/> class
        /// with the provided authenticator.
        /// </summary>
        /// <param name="authenticator">Google API authenticator used to obtain access token.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public GoogleCalendarService(OAuth2Authenticator authenticator)
        {
            this.authenticator =
                authenticator ?? throw new ArgumentNullException(nameof(authenticator));

            calendarServiceClient = new AsyncLazy<CalendarService>(InitializeApiClient);

            calendarSyncStates =
                new PooledSortedList<CalendarSyncState>(
                    Comparer<CalendarSyncState>.Create(
                        (x, y) => Comparer<string>.Default.Compare(x.CalendarId, y.CalendarId)));
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method will also refresh the token if it has expired. This is a cheating mechanism to do it,
        /// because we know this method is only called from settings. If user gets a failed sync due to expired
        /// token, we don't want to open a browser prompt out of the blue, so events query is not going to refresh
        /// the token, but simply fail. User can go to settings and 'edit' the account to refresh expired token.
        /// </remarks>
        public async Task<IEnumerable<CalendarInfo>> ListCalendarsAsync(CancellationToken cancellation)
        {
            var calendarService =
                await calendarServiceClient.GetValue(cancellation);

            var calendars = new List<CalendarInfo>();
            var calendarsRequest = calendarService.CalendarList.List();

            do
            {
                CalendarList response;

                try
                {
                    response = await calendarsRequest.ExecuteAsync(cancellation);
                }
                catch (TokenResponseException tokenError)
                {
                    if (OAuthInvalidGrantError.Equals(tokenError.Error?.Error, StringComparison.OrdinalIgnoreCase))
                    {
                        // If token has expired, we need to reset our client to authenticate again.
                        // Normally there will be a refresh token, which will be used to refresh the actual
                        // access token behind the scenes, but if refresh token expires as well (or otherwise
                        // becomes stale and invalid), then we would get an error and need to prompt user
                        // to authenticate again.
                        // Release old client first
                        calendarService.Dispose();

                        // Re-initialize the client to prompt for user credentials
                        calendarServiceClient =
                            new AsyncLazy<CalendarService>(InitializeApiClient);
                        calendarService =
                            await calendarServiceClient.GetValue(cancellation);

                        // Create new calendars request
                        var previousPageToken = calendarsRequest.PageToken;
                        calendarsRequest = calendarService.CalendarList.List();
                        calendarsRequest.PageToken = previousPageToken;

                        // Retry failed request
                        response = await calendarsRequest.ExecuteAsync(cancellation);
                    }
                    else
                    {
                        throw;
                    }
                }

                foreach (var calendar in response.Items)
                {
                    calendars.Add(
                        new CalendarInfo(
                            calendar.Id,
                            calendar.Summary,
                            ColorTranslator.FromHtml(calendar.BackgroundColor)));
                }

                calendarsRequest.PageToken = response.NextPageToken;
            }
            while (!string.IsNullOrEmpty(calendarsRequest.PageToken));
            
            return calendars;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CalendarEventInfo>> ListCalendarEventsAsync(
            string calendarId,
            DateTime windowStartTimeUtc,
            DateTime windowEndTimeUtc,
            CancellationToken cancellation)
        {
            var calendarService =
                await calendarServiceClient.GetValue(cancellation);

            // Retrieve earliest default reminder time for the calendar
            var defaultReminderInterval =
                (await calendarService.CalendarList.Get(calendarId).ExecuteAsync(cancellation))
                    .DefaultReminders?.Where(r => r.Method == NotificationReminderMethod).Max(r => r.Minutes);

            var todayDay = DateTime.UtcNow.Ticks / TimeSpan.TicksPerDay;

            var calendarSyncState = GetCalendarSyncState(calendarId, todayDay);

            var listEventsRequest =
                calendarService.Events.List(calendarId);

            // Retrieve recurring events as single occurences
            listEventsRequest.SingleEvents = true;
            listEventsRequest.TimeZone = "UTC";

            var clearCache = false;

            if (calendarSyncState.IncrementalSyncToken is not null)
            {
                // Perform incremental sync based on the sync token
                listEventsRequest.SyncToken = calendarSyncState.IncrementalSyncToken;
            }
            else
            {
                // Perform full sync for the currently active time interval, which is 10 minute before to 10 days ahead.
                // We need to remove all existing events in the cache, but we don't want to do it here. First we want
                // to make sure that we can actually sync the events without issues, so we set a flag here and clear up
                // the cache within the loop, once we start reading the results. Otherwise we will maintain our original
                // list of cached events, until the next successful sync.
                clearCache = true;

                listEventsRequest.TimeMinDateTimeOffset =
                    new DateTimeOffset(
                        calendarSyncState.LastSyncDay * TimeSpan.TicksPerDay - 10 * TimeSpan.TicksPerMinute,
                        TimeSpan.Zero);
                listEventsRequest.TimeMaxDateTimeOffset =
                    new DateTimeOffset(
                        (calendarSyncState.LastSyncDay + 10) * TimeSpan.TicksPerDay,
                        TimeSpan.Zero);
            }

            try
            {
                // Query events and merge them into our events cache
                await foreach (var calendarEvent
                    in new CalendarEventsEnumerable(listEventsRequest))
                {
                    // At this point we know we can at least start reading the events, so we can safely clear the cache.
                    // There is still a problem where next page in the paged result may fail, but we consider that a rare
                    // case, so it's not worth it to allocate a new buffer for it and we prefer to reuse an existing one.
                    if (clearCache)
                    {
                        calendarSyncState.CachedEvents.Clear();
                        clearCache = false;
                    }

                    // When an event information is present during incremental sync, there can be
                    // one of the following cases:
                    //  1) An event was deleted, then the start time will be empty.
                    //  2) An event has changed, then some of the fields will be different.
                    //
                    // Technically in second case we could update the fields in the existing event,
                    // but since we order the events in the cache by start time - it is easier to
                    // just remove it and add new one back. Hence, we just remove cached event in both
                    // of the cases, if this is an incremental sync and we get an event information.
                    if (calendarSyncState.IncrementalSyncToken is not null)
                    {
                        calendarSyncState.CachedEvents
                            .RemoveFirst(@event => @event.Id == calendarEvent.Id);
                    }

                    if (!"cancelled".Equals(calendarEvent.Status, StringComparison.OrdinalIgnoreCase)
                            && calendarEvent.Start != null)
                    {
                        calendarSyncState.CachedEvents.Add(
                            AsEventInfo(calendarEvent, defaultReminderInterval));
                    }
                }

                // The `CalendarEventsEnumerable` will update the request in place to pass back a sync token,
                // so we read it from there, if it is present
                calendarSyncState.IncrementalSyncToken = listEventsRequest.SyncToken;
            }
            catch (GoogleApiException apiException)
            {
                if (apiException.HttpStatusCode == HttpStatusCode.Gone)
                {
                    // Our incremental sync token is not valid anymore, we need to perform full sync
                    calendarSyncState.IncrementalSyncToken = null;
                    return await ListCalendarEventsAsync(calendarId, windowStartTimeUtc, windowEndTimeUtc, cancellation);
                }

                throw;
            }

            return GetEventsWithinRange(
                calendarSyncState.CachedEvents, windowStartTimeUtc, windowEndTimeUtc);
        }

        /// <summary>
        /// Disposes the Google Calendar client, if it was already initialized.
        /// </summary>
        public void Dispose()
        {
            if (calendarServiceClient.IsInitialized)
            {
                calendarServiceClient.GetValue(CancellationToken.None)
                    .Result.Dispose();
            }

            foreach (var syncState in calendarSyncStates)
            {
                syncState.Dispose();
            }

            calendarSyncStates.Dispose();
        }

        /// <summary>
        /// Retrives events in the given time interval from the ordered collection.
        /// </summary>
        /// <param name="events">Ordered colleciton of events.</param>
        /// <param name="windowStartTimeUtc">Lower bound (exclusive) for an event's end UTC time to filter by.</param>
        /// <param name="windowEndTimeUtc">Upper bound (exclusive) for an event's start UTC time to filter by.</param>
        /// <returns>Collection of events that start or end within the given time interval.</returns>
        private static IEnumerable<CalendarEventInfo> GetEventsWithinRange(
            IReadOnlyList<CalendarEventInfo> events,
            DateTime windowStartTimeUtc,
            DateTime windowEndTimeUtc)
        {
            for (var eventIndex = 0; eventIndex < events.Count; ++eventIndex)
            {
                var @event = events[eventIndex];

                if (@event.StartTimeUtc >= windowEndTimeUtc)
                {
                    // All events behind this point will be starting (and therefore ending)
                    // outside of the requested interval
                    break;
                }

                if (@event.EndTimeUtc > windowStartTimeUtc)
                {
                    yield return @event;
                }
            }
        }

        /// <summary>
        /// Converts Google Calendar API event data to our internal data structure.
        /// </summary>
        /// <param name="calendarEvent">Google Calendar API event data.</param>
        /// <param name="defaultReminderInterval">Default reminder interval set on calendar level.</param>
        /// <returns>Simplified internal event data structure.</returns>
        private static CalendarEventInfo AsEventInfo(Event calendarEvent, int? defaultReminderInterval)
        {
            var isAllDay = calendarEvent.Start.DateTimeDateTimeOffset is null;
            var startTime = AsDateTime(calendarEvent.Start);
            var endTime = AsDateTime(calendarEvent.End);

            if (isAllDay && startTime == endTime)
            {
                // If an all-day event reports the same start and end time (which will be just the date),
                // then we artificially advance end time to the end of that day
                endTime.AddDays(1);
            }

            // Pick the first configured UI (popup) notification for the event or use default
            var reminderIntervalMinutes =
                calendarEvent.Reminders?.Overrides?.Where(r => r.Method == NotificationReminderMethod).Max(r => r.Minutes)
                    ?? defaultReminderInterval;

            if (!reminderIntervalMinutes.HasValue)
            {
                // If there is no calendar default and nothing set on the event itself,
                // then we use 1 day for all-day events and 15 minutes for regular ones
                reminderIntervalMinutes = isAllDay ? 1440 : 15;
            }

            return new CalendarEventInfo(
                calendarEvent.Id,
                calendarEvent.Summary ?? Resources.NoTitle,
                calendarEvent.HtmlLink,
                GetConferenceUri(calendarEvent),
                startTime,
                endTime,
                TimeSpan.FromMinutes(reminderIntervalMinutes.Value));
        }

        /// <summary>
        /// Determines if there is a video conference associated with the event
        /// and extracts a join URI, if there is one.
        /// </summary>
        /// <param name="calendarEvent">Google Calendar API event data.</param>
        /// <returns>
        /// URI to join the video conference for the event, or null,
        /// if there is no video conference associated with the event.
        /// </returns>
        private static string? GetConferenceUri(Event calendarEvent)
        {
            // Check if there is a dedicated video conference link in the entry points
            if (calendarEvent.ConferenceData != null
                && calendarEvent.ConferenceData.EntryPoints != null)
            {
                foreach (var entryPoint in calendarEvent.ConferenceData.EntryPoints)
                {
                    if (entryPoint.EntryPointType == VideoConferenceEntryPointType)
                    {
                        return entryPoint.Uri;
                    }
                }
            }
            
            // Check if 'Location' contains a Zoom link
            if (!string.IsNullOrEmpty(calendarEvent.Location) && ZoomLinkRegex.IsMatch(calendarEvent.Location))
            {
                return calendarEvent.Location;
            }

            return null;
        }

        /// <summary>
        /// Converts Google Calendar API event's datetime structure to regular .NET <see cref="DateTime"/>.
        /// </summary>
        /// <remarks>
        /// For all-day events `DateTimeRaw` field will be empty, so we parse `Date` field instead.
        /// </remarks>
        /// <param name="eventDateTime">Google Calendar API event's datetime.</param>
        /// <returns>Regular .NET <see cref="DateTime"/>.</returns>
        private static DateTime AsDateTime(EventDateTime eventDateTime) =>
            string.IsNullOrEmpty(eventDateTime.DateTimeRaw)
                // For all-day events we want to start/end the day at local time, so we assume local
                // TODO: Better designtaion for all-day events that will be resilient to time zone changes?
                ? DateTime.ParseExact(
                    eventDateTime.Date,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal).ToUniversalTime()
                // For regular events we requested time stamps to be in UTC, so we assume they are in UTC
                : DateTime.ParseExact(
                    eventDateTime.DateTimeRaw,
                    "yyyy-MM-dd'T'HH:mm:ssK" ,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal).ToUniversalTime();

        /// <summary>
        /// Retrieves synchronization state for the calendar with the given identifier.
        /// Additionally, old states will be trimmed to prevent memory leaks from removed calendars.
        /// </summary>
        /// <param name="calendarId">Identifier of the calendar to get synchronization state for.</param>
        /// <param name="todayDay">Today's day from '01-01-0001'.</param>
        /// <returns>Synchronization state for the requested calendar.</returns>
        private CalendarSyncState GetCalendarSyncState(string calendarId, long todayDay)
        {
            CalendarSyncState? requestedState = null;

            for (var stateIndex = 0; stateIndex < calendarSyncStates.Count; ++stateIndex)
            {
                if (calendarSyncStates[stateIndex].CalendarId == calendarId)
                {
                    requestedState = calendarSyncStates[stateIndex];
                }
                else if (todayDay - calendarSyncStates[stateIndex].LastSyncDay > 2)
                {
                    // Trim states for calendars that were not synchronized in last 2 days
                    calendarSyncStates.RemoveAt(stateIndex--);
                }
            }

            if (requestedState == null)
            {
                // New calendar requested - add new sync state
                requestedState = new CalendarSyncState(calendarId);
                calendarSyncStates.Add(requestedState);
            }

            if (requestedState.LastSyncDay < todayDay)
            {
                // If it is a new day, then we force full sync for the new time interval
                requestedState.LastSyncDay = todayDay;
                requestedState.IncrementalSyncToken = null;
            }

            return requestedState;
        }

        /// <summary>
        /// Initializes Google Calendar API client.
        /// </summary>
        /// <param name="cancellation">Cancellation token to stop initialization.</param>
        /// <returns>A <see cref="Task{TResult}"/> that asynchronously initializes the client.</returns>
        private async Task<CalendarService> InitializeApiClient(CancellationToken cancellation) =>
            new CalendarService(
                new BaseClientService.Initializer
                {
                    ApplicationName = "Calendaro .NET",
                    HttpClientInitializer = await authenticator.Authenticate(cancellation),
                });
    }
}
