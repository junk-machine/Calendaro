using Calendaro.Abstractions;
using Calendaro.GoogleCalendar;
using Calendaro.Settings;

namespace Calendaro.CalendarServices
{
    /// <summary>
    /// Factory that creates clients for supported calendar services.
    /// </summary>
    internal sealed class KnownCalendarServicesFactory : ICalendarServicesFactory
    {
        /// <summary>
        /// Path to store access tokens needed to access different calendar services.
        /// This should be a secure location, available to the current user only.
        /// </summary>
        private readonly string tokenStoragePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="KnownCalendarServicesFactory"/> class.
        /// </summary>
        /// <param name="tokenStoragePath">Path to store access tokens needed to access different calendar services.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public KnownCalendarServicesFactory(string tokenStoragePath)
        {
            this.tokenStoragePath =
                tokenStoragePath ?? throw new ArgumentNullException(nameof(tokenStoragePath));
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException">Unsupported account type.</exception>
        public ICalendarService Create(CalendarServiceType calendarServiceType, string accountId) =>
            calendarServiceType switch
            {
                CalendarServiceType.Google =>
                    new GoogleCalendarService(new OAuth2Authenticator(accountId, tokenStoragePath)),
                _ => throw new ArgumentOutOfRangeException(nameof(calendarServiceType)),
            };
    }
}
