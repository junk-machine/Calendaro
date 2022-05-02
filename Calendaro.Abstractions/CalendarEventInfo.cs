namespace Calendaro.Abstractions
{
    /// <summary>
    /// Describes calendar event.
    /// </summary>
    public sealed class CalendarEventInfo
    {
        /// <summary>
        /// Gets the identifier of the event.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the user-friendly name of the event.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the URI to open full event description.
        /// </summary>
        public string? EventUri { get; }

        /// <summary>
        /// Gets the URI to join the conference, if available.
        /// </summary>
        public string? ConferenceUri { get; }

        /// <summary>
        /// Gets the UTC start time of the event.
        /// </summary>
        public DateTime StartTimeUtc { get; }

        /// <summary>
        /// Gets the UTC end time of the event.
        /// </summary>
        public DateTime EndTimeUtc { get; }

        /// <summary>
        /// Gets the time interval before the event start at which notification should be provided to the user.
        /// </summary>
        public TimeSpan ReminderInterval { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarEventInfo"/> class
        /// with the provided identifier, title, start and end time, and reminder interval.
        /// </summary>
        /// <param name="id">Identifier of the event.</param>
        /// <param name="title">User-friendly name of the event.</param>
        /// <param name="eventUri">URI to open full event description.</param>
        /// <param name="conferenceUri">URI to join the conference, if available.</param>
        /// <param name="startTimeUtc">UTC start time of the event.</param>
        /// <param name="endTimeUtc">UTC end time of the event.</param>
        /// <param name="reminderInterval">Time interval before the event start at which notification should be provided to the user.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public CalendarEventInfo(
            string id,
            string title,
            string eventUri,
            string? conferenceUri,
            DateTime startTimeUtc,
            DateTime endTimeUtc,
            TimeSpan reminderInterval)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            EventUri = eventUri;
            ConferenceUri = conferenceUri;
            StartTimeUtc = startTimeUtc;
            EndTimeUtc = endTimeUtc;
            ReminderInterval = reminderInterval;
        }

        /// <summary>
        /// Compares identifier of this event to the identifier
        /// of <paramref name="obj"/> event.
        /// </summary>
        /// <param name="obj">Another instance to compare to.</param>
        /// <returns>true if event identifiers are the same, otherwise false.</returns>
        public override bool Equals(object? obj) =>
            obj is CalendarEventInfo other && Id == other.Id;

        /// <summary>
        /// Gets the hash code of the event identifier.
        /// </summary>
        /// <returns>Hash code of the event identifier.</returns>
        public override int GetHashCode() => Id.GetHashCode();
    }
}
