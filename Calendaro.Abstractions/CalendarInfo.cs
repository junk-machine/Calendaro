using System.Drawing;

namespace Calendaro.Abstractions
{
    /// <summary>
    /// Describes a calendar.
    /// </summary>
    public sealed class CalendarInfo : ICalendarInfo
    {
        /// <summary>
        /// Gets the identifier of the calendar.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the user-friendly name of the calendar.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the color associated with the calendar.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarInfo"/> class
        /// with the provided calendar identifier and name.
        /// </summary>
        /// <param name="id">Identifier of the calendar</param>
        /// <param name="name">User-friendly name of the calendar.</param>
        /// <param name="color">Color associated with the calendar.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public CalendarInfo(string id, string name, Color color)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Color = color;
        }

        /// <summary>
        /// Compares identifier of this calendar to the identifier
        /// of <paramref name="obj"/> calendar.
        /// </summary>
        /// <param name="obj">Another instance to compare to.</param>
        /// <returns>true if calendar identifiers are the same, otherwise false.</returns>
        public override bool Equals(object? obj) =>
            obj is CalendarInfo other && Id == other.Id;

        /// <summary>
        /// Gets the hash code of the calendar identifier.
        /// </summary>
        /// <returns>Hash code of the calendar identifier.</returns>
        public override int GetHashCode() => Id.GetHashCode();
    }
}
