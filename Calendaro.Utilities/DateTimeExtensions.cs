namespace Calendaro.Utilities
{
    /// <summary>
    /// Defines extension methods for the <see cref="DateTime"/> structure.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Snaps the provided <paramref name="dateTime"/> to the to the nearest <paramref name="flooringInterval"/> below the provided timestamp.
        /// </summary>
        /// <param name="dateTime">Timestamp to floor.</param>
        /// <param name="flooringInterval">Time interval to floor to. This can be 1 minute, 15 minutes, 1 hour, etc.</param>
        /// <returns>New timestamp that is adjusted to the nearest interval.</returns>
        public static DateTime Floor(this DateTime dateTime, TimeSpan flooringInterval) =>
            new(dateTime.Ticks / flooringInterval.Ticks * flooringInterval.Ticks);

        /// <summary>
        /// Snaps the provided <paramref name="dateTime"/> to the to the nearest minute below the provided timestamp.
        /// </summary>
        /// <param name="dateTime">Timestamp to floor.</param>
        /// <returns>New timestamp that is adjusted to the nearest minute.</returns>
        public static DateTime FloorMinutes(this DateTime dateTime) =>
            new(dateTime.Ticks / TimeSpan.TicksPerMinute * TimeSpan.TicksPerMinute);
    }
}
