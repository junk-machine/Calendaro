using Calendaro.Abstractions;
using Calendaro.Properties;
using Calendaro.Settings;

namespace Calendaro.EventsTracking
{
    /// <summary>
    /// An error that is thrown when synchronization fails for the calendar.
    /// </summary>
    internal sealed class CalendarSyncException : Exception
    {
        /// <summary>
        /// Gets the information about the calendar service account that failed synchronization.
        /// </summary>
        public ICalendarAccountInfo Account { get; }

        /// <summary>
        /// Gets the information about the calendar that failed synchronization.
        /// </summary>
        public ICalendarInfo Calendar { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarSyncException"/> class
        /// with the provided account and calendar information.
        /// </summary>
        /// <param name="account">Information about the calendar service account that failed synchronization.</param>
        /// <param name="calendar">Information about the calendar that failed synchronization.</param>
        /// <param name="syncError">Calendar synchronization error.</param>
        public CalendarSyncException(ICalendarAccountInfo account, ICalendarInfo calendar, Exception syncError)
            : base(string.Format(Resources.CalendarSyncErrorMessage, calendar.Id, account.AccountId), syncError)
        {
            Account =
                account ?? throw new ArgumentNullException(nameof(account));

            Calendar =
                calendar ?? throw new ArgumentNullException(nameof(calendar));
        }
    }
}
