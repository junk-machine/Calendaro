using System.Drawing;

namespace Calendaro.Abstractions
{
    /// <summary>
    /// Carries information about the calendar.
    /// </summary>
    public interface ICalendarInfo
    {
        /// <summary>
        /// Gets the identifier of the calendar.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the user-friendly name of the calendar.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the color associated with the calendar.
        /// </summary>
        Color Color { get; }
    }
}
