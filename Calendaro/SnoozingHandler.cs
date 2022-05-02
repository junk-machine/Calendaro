using Calendaro.Abstractions;
using Calendaro.Properties;
using Calendaro.UI;

namespace Calendaro
{
    /// <summary>
    /// Handles different aspects of events snoozing.
    /// </summary>
    internal sealed class SnoozingHandler
    {
        /// <summary>
        /// List of standard snoozing intervals.
        /// </summary>
        /// <remarks>
        /// This list will be considered when computing applicable 'before start' options
        /// as well as regular 'snooze for' oprtions.
        /// </remarks>
        private static readonly List<TimeSpan> StandardIntervals =
            new()
            {
                TimeSpan.FromDays(14),
                TimeSpan.FromDays(7),
                TimeSpan.FromDays(5),
                TimeSpan.FromDays(2),
                TimeSpan.FromDays(1),
                TimeSpan.FromHours(18),
                TimeSpan.FromHours(12),
                TimeSpan.FromHours(8),
                TimeSpan.FromHours(6),
                TimeSpan.FromHours(2),
                TimeSpan.FromHours(1),
                TimeSpan.FromMinutes(45),
                TimeSpan.FromMinutes(30),
                TimeSpan.FromMinutes(15),
                TimeSpan.FromMinutes(5),
            };

        /// <summary>
        /// Singleton instance of the provider.
        /// </summary>
        public static readonly SnoozingHandler Instance = new SnoozingHandler();

        /// <summary>
        /// Suggests standard snoozing intervals for the given event.
        /// </summary>
        /// <param name="event">An event to suggest standard snoozing intervals for.</param>
        /// <returns>List of suggested snoozing options with associated next reminder time.</returns>
        public IReadOnlyList<ListControlItem<DateTime>> SuggestIntervals(CalendarEventInfo @event)
        {
            var utcNow = DateTime.UtcNow;
            var suggestedintervals = new List<ListControlItem<DateTime>>();

            // First, list 'before start' options
            for (var intervalIndex = 0; intervalIndex < StandardIntervals.Count; ++intervalIndex)
            {
                var snoozingInterval = StandardIntervals[intervalIndex];

                if (@event.StartTimeUtc - snoozingInterval > utcNow)
                {
                    suggestedintervals.Add(
                        new ListControlItem<DateTime>(
                            string.Format(Resources.BeforeStartSuffix, snoozingInterval.AsRemainingTimeString()),
                            @event.StartTimeUtc - snoozingInterval));
                }
            }

            // Then add an option to snooze until start.
            // Events that are starting within 10 seconds are considered happening now,
            // so we don't display this option for them either.
            if (utcNow <= @event.StartTimeUtc.AddSeconds(-10))
            {
                suggestedintervals.Add(
                    new ListControlItem<DateTime>(
                        Resources.SnoozeUntilEventStarts,
                        @event.StartTimeUtc));
            }

            // Finally, add 'snooze for' options (these are displayed in reverse)
            for (var intervalIndex = StandardIntervals.Count - 1; intervalIndex > 0; --intervalIndex)
            {
                var snoozingInterval = StandardIntervals[intervalIndex];

                if (utcNow + snoozingInterval >= @event.EndTimeUtc)
                {
                    // Do not suggest to snooze past event end time
                    break;
                }

                suggestedintervals.Add(
                    new ListControlItem<DateTime>(
                        snoozingInterval.AsRemainingTimeString(),
                        utcNow + snoozingInterval));
            }

            return suggestedintervals;
        }
    }
}
