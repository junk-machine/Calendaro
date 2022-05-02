namespace Calendaro.Utilities
{
    /// <summary>
    /// Defines extension methods for the <see cref="IList{T}"/> interface.
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// Adds multiple <paramref name="items"/> to the <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T">Type of items to add.</typeparam>
        /// <param name="list">List to add items to.</param>
        /// <param name="items">Items that should be added to the list.</param>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }
    }
}
