using Calendaro.Abstractions;

namespace Calendaro.EventsTracking
{
    /// <summary>
    /// Describes an event that is managed by the <see cref="EventsManager"/>.
    /// </summary>
    internal sealed class TrackedEvent
    {
        /// <summary>
        /// Gets the information about the calendar to which the event belongs.
        /// </summary>
        public CalendarInfo Calendar { get; }

        /// <summary>
        /// Gets the information about the event.
        /// </summary>
        public CalendarEventInfo Event { get; }

        /// <summary>
        /// Gets or sets the time at which reminder should be displayed for this event.
        /// </summary>
        /// <remarks>
        /// Initially this field will be set based on the configured reminder interval.
        /// This should be updated when event is snoozed and set to null when dismissed.
        /// </remarks>
        public DateTime? RemindAtUtc { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackedEvent"/> class
        /// with the provided calendar and event information.
        /// </summary>
        /// <param name="calendar">Information about the calendar to which the event belongs.</param>
        /// <param name="event">Information about the event.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public TrackedEvent(CalendarInfo calendar, CalendarEventInfo @event)
        {
            Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));
            Event = @event ?? throw new ArgumentNullException(nameof(@event));

            RemindAtUtc = @event.StartTimeUtc - @event.ReminderInterval;
        }

        /// <summary>
        /// Compares this instance of the <see cref="TrackedEvent"/> to the <paramref name="obj"/>
        /// based on the calendar and event identifiers.
        /// </summary>
        /// <param name="obj">Another instance to compare to.</param>
        /// <returns>true if this instance is equal to the provided one, otherwise false.</returns>
        public override bool Equals(object? obj) =>
            obj is TrackedEvent other
            && Calendar.Id == other.Calendar.Id
            && Event.Id == Event.Id;

        /// <summary>
        /// Computes hash code for this based on the calendar and event identifiers.
        /// </summary>
        /// <returns>Hash code for this instance.</returns>
        public override int GetHashCode() =>
            Calendar.Id.GetHashCode() ^ Event.Id.GetHashCode();
    }
}
