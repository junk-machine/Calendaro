namespace Calendaro.UI
{
    /// <summary>
    /// Represents an item in the list control that has display name and an associated value.
    /// </summary>
    /// <typeparam name="TValue">Type of the associated value.</typeparam>
    internal sealed class ListControlItem<TValue>
    {
        /// <summary>
        /// Gets the display name of the list item.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets the value associated with the list item.
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListControlItem{TValue}"/> class
        /// with the provided display name and value.
        /// </summary>
        /// <param name="displayName">Display name of the list item.</param>
        /// <param name="value">Value associated with the list item.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public ListControlItem(string displayName, TValue value)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Retrieves display name of the item.
        /// </summary>
        /// <remarks>
        /// This allows to bind the <see cref="ListControlItem{TValue}"/> to a list control
        /// without specifying the <see cref="ListControl.DisplayMember"/>.
        /// </remarks>
        /// <returns>Display name of the item.</returns>
        public override string ToString()
        {
            return DisplayName;
        }

        /// <summary>
        /// Compares this instance to <paramref name="obj"/> based on the <see cref="ListControlItem{TValue}.Value"/>
        /// property.
        /// </summary>
        /// <param name="obj">Other instance to compare to.</param>
        /// <returns>true if both items have the same value, otherwise false.</returns>
        public override bool Equals(object? obj) =>
            obj is ListControlItem<TValue> otherItem
                && ((Value is not null && Value.Equals(otherItem.Value))
                    || (Value is null && otherItem.Value is null));

        /// <summary>
        /// Computes hash code based on the <see cref="ListControlItem{TValue}.Value"/> property.
        /// </summary>
        /// <returns>Hash code of the <see cref="ListControlItem{TValue}.Value"/>.</returns>
        public override int GetHashCode() =>
            Value is null ? 0 : Value.GetHashCode();
    }
}
