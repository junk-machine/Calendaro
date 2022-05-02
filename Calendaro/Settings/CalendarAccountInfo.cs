namespace Calendaro.Settings
{
    /// <summary>
    /// Carries information about the calendar service account.
    /// </summary>
    internal interface ICalendarAccountInfo
    {
        /// <summary>
        /// Gets the type of the calendar service.
        /// </summary>
        public CalendarServiceType CalendarServiceType { get; }

        /// <summary>
        /// Gets the identifier of the account.
        /// </summary>
        public string AccountId { get; }
    }
}
