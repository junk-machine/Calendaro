using Calendaro.Abstractions;
using Calendaro.Settings;

namespace Calendaro.CalendarServices
{
    /// <summary>
    /// Factory that creates clients for different calendar services.
    /// </summary>
    internal interface ICalendarServicesFactory
    {
        /// <summary>
        /// Creates an instance of the calendar service client that retrieves events for the given account.
        /// </summary>
        /// <param name="calendarServiceType">Type of the calendar service.</param>
        /// <param name="accountId">Identifier of the calendar service account.</param>
        /// <returns>A calendar service client that can be used to access calendars for the given account.</returns>
        ICalendarService Create(CalendarServiceType calendarServiceType, string accountId);
    }
}
