using Newtonsoft.Json;

namespace Calendaro.Settings
{
    /// <summary>
    /// Describes application configuration.
    /// </summary>
    internal class CalendaroSettings
    {
        /// <summary>
        /// Gets the list of calendar accounts that should be synchronized.
        /// </summary>
        public IList<CalendarAccountConfiguration> AccountsConfiguration { get; } =
            new List<CalendarAccountConfiguration>();

        /// <summary>
        /// Gets or sets the value indicating whether application should start when user signs in.
        /// </summary>
        public bool AutoStart { get; set; }

        /// <summary>
        /// Gets or sets the number of hours (from now) to pre-load events for.
        /// </summary>
        public int LookaheadHours { get; set; } = 48;

        /// <summary>
        /// Creates a deep copy of the settings instance.
        /// </summary>
        /// <returns>Copy of the settings.</returns>
        public CalendaroSettings Copy()
        {
            // TODO: Use reflection to make it more efficient?
            return
                JsonConvert.DeserializeObject<CalendaroSettings>(JsonConvert.SerializeObject(this))
                    ?? new CalendaroSettings();
        }
    }
}
