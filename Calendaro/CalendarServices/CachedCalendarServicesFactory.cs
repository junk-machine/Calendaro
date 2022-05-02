using Calendaro.Abstractions;
using Calendaro.Settings;
using System.Collections.Concurrent;

namespace Calendaro.CalendarServices
{
    /// <summary>
    /// Calendar service clients factory that caches clients created by the underlying factory.
    /// </summary>
    internal sealed class CachedCalendarServicesFactory : ICalendarServicesFactory
    {
        /// <summary>
        /// Factory used to create actual calendar service clients.
        /// </summary>
        private readonly ICalendarServicesFactory underlyingFactory;

        /// <summary>
        /// Cache of the calendar service clients.
        /// </summary>
        private readonly ConcurrentDictionary<CacheKey, ICalendarService> clientsCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCalendarServicesFactory"/> class
        /// with the provided underlying factory.
        /// </summary>
        /// <param name="underlyingFactory">Factory used to create actual calendar service clients.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public CachedCalendarServicesFactory(ICalendarServicesFactory underlyingFactory)
        {
            this.underlyingFactory =
                underlyingFactory ?? throw new ArgumentNullException(nameof(underlyingFactory));

            clientsCache = new ConcurrentDictionary<CacheKey, ICalendarService>();
        }

        /// <inheritdoc/>
        /// <summary>
        /// Checks if there is already a client available for the given service type and account identifier pair.
        /// If there is none, than new one will be created using underlying factory.
        /// </summary>
        public ICalendarService Create(CalendarServiceType calendarServiceType, string accountId) =>
            // TODO: Prune cache every once in a while? Technically this is leaking memory, but in reality
            //       we don't anticipate that many accounts being added and removed continuously.
            clientsCache.GetOrAdd(new CacheKey(calendarServiceType, accountId), CreateClient);

        /// <summary>
        /// Creates new calendar service client using underlying factory.
        /// </summary>
        /// <param name="configuration">Configuration for the client to be created.</param>
        /// <returns>New calendar service client for the requested service type and account.</returns>
        private ICalendarService CreateClient(CacheKey configuration) =>
            underlyingFactory.Create(configuration.CalendarServiceType, configuration.AccountId);

        /// <summary>
        /// Key used in the calendar service clients cache dictionary.
        /// </summary>
        private struct CacheKey
        {
            /// <summary>
            /// Type of the calendar service.
            /// </summary>
            public CalendarServiceType CalendarServiceType;

            /// <summary>
            /// Identifier of the account.
            /// </summary>
            public string AccountId;

            /// <summary>
            /// Initializes new instance of the <see cref="CacheKey"/> struct
            /// with the provided calendar service type and account identifier.
            /// </summary>
            /// <param name="calendarServiceType">Type of the calendar service.</param>
            /// <param name="accountId">Identifier of the account.</param>
            public CacheKey(CalendarServiceType calendarServiceType, string accountId)
            {
                CalendarServiceType = calendarServiceType;
                AccountId = accountId;
            }

            /// <inheritdoc/>
            public override bool Equals(object? obj) =>
                obj is CacheKey other
                && other.CalendarServiceType == CalendarServiceType
                && other.AccountId == AccountId;

            /// <inheritdoc/>
            public override int GetHashCode() =>
                CalendarServiceType.GetHashCode() ^ AccountId.GetHashCode();
        }
    }
}
