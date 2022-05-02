using Calendaro.Settings;

namespace Calendaro.UI
{
    /// <summary>
    /// Calendar icons provider that generates rectangles filled with solid color
    /// matching the color of the calendar.
    /// </summary>
    internal sealed class SolidColorCalendarIconsProvider : ICalendarIconsProvider
    {
        /// <summary>
        /// Padding from the borders to the actual icon.
        /// </summary>
        private readonly int iconPadding;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolidColorCalendarIconsProvider"/> class
        /// with the provided icon padding.
        /// </summary>
        /// <param name="padding">Number of pixels from the image border to the icon.</param>
        public SolidColorCalendarIconsProvider(int padding)
        {
            iconPadding = padding;
        }

        /// <inheritdoc/>
        public void PopulateImageList(
            IEnumerable<CalendarAccountConfiguration> calendarAccounts,
            ImageList imageList)
        {
            var iconSize =
                new Rectangle(
                    iconPadding,
                    iconPadding,
                    imageList.ImageSize.Width - iconPadding * 2,
                    imageList.ImageSize.Height - iconPadding * 2);

            imageList.Images.Add(
                "cal",
                GenerateCalendarIcon(imageList.ImageSize, iconSize, Color.Pink));

            foreach (var calendarAccount in calendarAccounts)
            {
                foreach (var calendar in calendarAccount.Calendars)
                {
                    // TODO: Use compound ID - { account, calendar }?
                    imageList.Images.Add(
                        calendar.Id,
                        GenerateCalendarIcon(imageList.ImageSize, iconSize, calendar.Color));
                }
            }
        }

        /// <summary>
        /// Generates a rectangular icon for the calendar with given color.
        /// </summary>
        /// <param name="imageSize">Size of the image to generate.</param>
        /// <param name="iconBounds">Bounds within the image where rectangle icon should be drawn.</param>
        /// <param name="calendarColor">Color of the calendar.</param>
        /// <returns>An icon for the calendar with given color.</returns>
        private Image GenerateCalendarIcon(Size imageSize, Rectangle iconBounds, Color calendarColor)
        {
            var calendarIcon = new Bitmap(imageSize.Width, imageSize.Height);

            using (var graphics = Graphics.FromImage(calendarIcon))
            {
                graphics.FillRectangle(new SolidBrush(calendarColor), iconBounds);
            }

            return calendarIcon;
        }
    }
}
