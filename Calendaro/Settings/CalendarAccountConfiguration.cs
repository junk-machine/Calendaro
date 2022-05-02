using Calendaro.Abstractions;

namespace Calendaro.Settings
{
    /// <summary>
    /// Describes configuration of the calendar account.
    /// </summary>
    internal sealed class CalendarAccountConfiguration : ICalendarAccountInfo
    {
        /// <summary>
        /// Gets or sets the type of the calendar service.
        /// </summary>
        public CalendarServiceType CalendarServiceType { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the account.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets the list of calendars that should be synchronized.
        /// </summary>
        public IList<CalendarInfo> Calendars { get; } = new List<CalendarInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarAccountConfiguration"/> class
        /// with the provided calendar service type and account identifier.
        /// </summary>
        /// <param name="calendarServiceType">Type of the calendar service.</param>
        /// <param name="accountId">Identifier of the calendar service account.</param>
        public CalendarAccountConfiguration(
            CalendarServiceType calendarServiceType,
            string accountId)
        {
            CalendarServiceType = calendarServiceType;
            AccountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
        }
    }
}
