using Calendaro.Properties;

namespace Calendaro
{
    /// <summary>
    /// Defines extension methods for the <see cref="TimeSpan"/> structure.
    /// </summary>
    internal static class TimeSpanExtensions
    {
        /// <summary>
        /// Generates a user-friendly string that reflects time remaining before the event.
        /// </summary>
        /// <param name="remainingTimeBeforeStart">Time interval before event starts. This can be negative, if event is already ongoing.</param>
        /// <returns>String representation of the remaining time interval.</returns>
        public static string AsRemainingTimeString(this TimeSpan remainingTimeBeforeStart)
        {
            var isOverdue = remainingTimeBeforeStart.Ticks < 0;

            if (isOverdue)
            {
                // Normalize time interval to have positive value
                remainingTimeBeforeStart = remainingTimeBeforeStart.Negate();
            }

            // Display 'Now' in the interval between 10 seconds before and 1 minute overdue
            if (remainingTimeBeforeStart.TotalSeconds < 10
                || isOverdue && remainingTimeBeforeStart.TotalMinutes < 1)
            {
                return Resources.Now;
            }

            // Round time interval to nearest minute
            remainingTimeBeforeStart =
                TimeSpan.FromMinutes(
                    isOverdue
                        ? Math.Truncate(remainingTimeBeforeStart.TotalMinutes)
                        : Math.Ceiling(remainingTimeBeforeStart.TotalMinutes));

            // We want to round to the nearest unit, if we are over 1.
            // If we are below 1, then we will break it down with better granularity:
            // days to hours, hours to minutes, hence want to have number of bigger units at 0.
            var totalWeeks = remainingTimeBeforeStart.TotalDays / 7;
            totalWeeks = totalWeeks >= 1 ? Math.Round(totalWeeks) : 0;

            var totalDays = remainingTimeBeforeStart.TotalDays >= 1 ? Math.Round(remainingTimeBeforeStart.TotalDays) : 0;
            var totalHours = remainingTimeBeforeStart.TotalHours >= 1 ? Math.Round(remainingTimeBeforeStart.TotalHours) : 0;
            var totalMinutes = remainingTimeBeforeStart.TotalMinutes >= 1 ? Math.Round(remainingTimeBeforeStart.TotalMinutes) : 0;

            var baseInterval =
                totalWeeks >= 2 ? string.Format(Resources.WeeksInterval, totalWeeks)
                : totalWeeks >= 1 ? Resources.WeekInterval
                : totalDays >= 2 ? string.Format(Resources.DaysInterval, totalDays)
                : totalDays >= 1 ? Resources.DayInterval
                : totalHours >= 2 ? string.Format(Resources.HoursInterval, totalHours)
                : totalHours >= 1 ? Resources.HourInterval
                : totalMinutes >= 2 ? string.Format(Resources.MinutesInterval, totalMinutes)
                : Resources.MinuteInterval;

            return isOverdue
                ? string.Format(Resources.OverdueSuffix, baseInterval)
                : baseInterval;
        }
    }
}
