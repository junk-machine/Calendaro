using Calendaro.Settings;

namespace Calendaro.UI
{
    /// <summary>
    /// Provides icons for calendars.
    /// </summary>
    internal interface ICalendarIconsProvider
    {
        /// <summary>
        /// Adds calendar icons to the given image collection.
        /// </summary>
        /// <param name="calendarAccounts">Collection of calendar service accounts to generate icons for.</param>
        /// <param name="imageList">Image list to add calendar icons to.</param>
        void PopulateImageList(
            IEnumerable<CalendarAccountConfiguration> calendarAccounts,
            ImageList imageList);
    }
}
