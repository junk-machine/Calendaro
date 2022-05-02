using System.Buffers;
using System.Collections;

namespace Calendaro.Utilities
{
    /// <summary>
    /// List of items that are stored in a sorted order in an array borrowed
    /// from the <see cref="ArrayPool{T}"/>.
    /// </summary>
    /// <remarks>
    /// While this implementation allows multiple items in the same bucket, APIs like
    /// <see cref="Remove(T)"/> and <see cref="FindIndex(T)"/> may remove any arbitrary
    /// element within the bucket. We could perform reference equality comparison, but this
    /// is not important for our scenarios, so we don't do secondary lookup within each bucket
    /// based on reference equality.
    /// </remarks>
    public sealed class PooledSortedList<T> : ICollection<T>, IReadOnlyList<T>, IDisposable
    {
        /// <summary>
        /// Comparer used to order items.
        /// </summary>
        private readonly IComparer<T> itemsComparer;

        /// <summary>
        /// Items in a sorted order.
        /// </summary>
        private T[] items;

        /// <inheritdoc/>
        public T this[int index]
        {
            get
            {
                if (index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return items[index];
            }
        }

        /// <inheritdoc/>
        public int Count { get; private set; }

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledSortedList{T}"/> class
        /// with the default items comparer.
        /// </summary>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public PooledSortedList()
            : this(Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PooledSortedList{T}"/> class
        /// with the provided items comparers.
        /// </summary>
        /// <param name="itemsComparer">Comparer used to order items.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public PooledSortedList(IComparer<T> itemsComparer)
        {
            this.itemsComparer =
                itemsComparer ?? throw new ArgumentNullException(nameof(itemsComparer));

            items = ArrayPool<T>.Shared.Rent(4);
        }

        /// <inheritdoc/>
        public void Add(T item)
        {
            var newItemIndex = SuggestItemIndex(item);

            var sourceItems = items;
            var targetItems = items;

            // Ensure array has enough room for one more element
            if (Count >= items.Length)
            {
                // Borrow new, bigger array and set it as a new target
                targetItems =
                    ArrayPool<T>.Shared.Rent(
                        Math.Min(items.Length * 2, int.MaxValue));
            }

            Array.Copy(sourceItems, 0, targetItems, 0, newItemIndex);
            Array.Copy(sourceItems, newItemIndex, targetItems, newItemIndex + 1, Count - newItemIndex);

            targetItems[newItemIndex] = item;
            ++Count;

            if (sourceItems != targetItems)
            {
                // If new array was borrowed, then we need to return the old one
                ArrayPool<T>.Shared.Return(items);
                items = targetItems;
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            Array.Clear(items, 0, Count);
            Count = 0;
        }

        /// <inheritdoc/>
        public bool Contains(T item) =>
            FindIndex(item) >= 0;

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array.Length <= arrayIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            Array.Copy(items, 0, array, arrayIndex, Math.Min(Count, array.Length - arrayIndex));
        }

        /// <summary>
        /// Searches for the first item in the list that matches given predicate.
        /// </summary>
        /// <param name="match">Predicate used to find matching item.</param>
        /// <returns>An item, or deafult value of the <typeparamref name="T"/>, if item is not found.</returns>
        public T? Find(Predicate<T> match)
        {
            for (var itemIndex = 0; itemIndex < Count; ++itemIndex)
            {
                if (match(items[itemIndex]))
                {
                    return items[itemIndex];
                }
            }

            return default;
        }

        /// <summary>
        /// Searches for the given item in the list.
        /// </summary>
        /// <param name="item">Item to find.</param>
        /// <returns>An index of the item, or -1, if item is not found.</returns>
        public int FindIndex(T item)
        {
            if (Count <= 0)
            {
                return -1;
            }

            var suggestedIndex = SuggestItemIndex(item);

            return
                itemsComparer.Compare(item, items[suggestedIndex]) == 0
                ? suggestedIndex
                : -1;
        }

        /// <summary>
        /// Searches for the item in the list that matches given predicate.
        /// </summary>
        /// <param name="match">Predicate used to find matching item.</param>
        /// <returns>An index of the item, or -1, if item is not found.</returns>
        public int FindIndex(Predicate<T> match)
        {
            for (var itemIndex = 0; itemIndex < Count; ++itemIndex)
            {
                if (match(items[itemIndex]))
                {
                    return itemIndex;
                }
            }

            return -1;
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            var itemIndex = FindIndex(item);

            if (itemIndex >= 0)
            {
                RemoveAt(itemIndex);
            }

            return itemIndex >= 0;
        }

        /// <summary>
        /// Searches for the first item in the list that matches given predicate and removes it.
        /// </summary>
        /// <param name="match">Predicate used to find matching item.</param>
        /// <returns>true if item was found, otherwise false.</returns>
        public bool RemoveFirst(Predicate<T> match)
        {
            for (var itemIndex = 0; itemIndex < Count; ++itemIndex)
            {
                if (match(items[itemIndex]))
                {
                    RemoveAt(itemIndex);
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            // At the end of the method we will truncate last element, so if index
            // points to the last element, then we don't need to do anything else
            if (index <= items.Length - 1)
            {
                Array.Copy(items, index + 1, items, index, Count - index - 1);
            }

            // Clear last element, to make sure it can be garbage-collected
            // This is only needed, if we are removing last element, but just to keep
            // an array in clean state, we do this all the time
            Array.Clear(items, --Count, 1);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            for (var itemIndex = 0; itemIndex < Count; ++itemIndex)
            {
                yield return items[itemIndex];
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Returns underlying array to the shared <see cref="ArrayPool{T}"/>.
        /// </summary>
        public void Dispose()
        {
            ArrayPool<T>.Shared.Return(items);
        }

        /// <summary>
        /// Determines an index where item should be inserted by performing binary search.
        /// </summary>
        /// <param name="item">An item to find a slot for.</param>
        /// <returns>An index in to the <see cref="items"/> array where given item should be inserted.</returns>
        private int SuggestItemIndex(T item)
        {
            int targetIndex = 0;
            var lowBoundary = 0;
            var highBoundary = Count - 1;
            
            while (lowBoundary <= highBoundary)
            {
                targetIndex =
                    lowBoundary + ((highBoundary - lowBoundary) >> 1);

                var comparisonResult =
                    itemsComparer.Compare(item, items[targetIndex]);

                if (comparisonResult == 0)
                {
                    // Found the same element, will insert at its index
                    // as we don't guarantee an order within same bucket
                    return targetIndex;
                }
                else if (comparisonResult > 0)
                {
                    // On the very last iteration, if there is no mathcing element found and
                    // both bounadries are equal, there are two possible outcomes:
                    //  1) insert element after the current `targetIndex` (new item is greater than item at `targetIndex`)
                    //  2) insert element at the `targetIndex` (new item is less than item at `targetIndex`)
                    // This branch covers (1) by adjusting `targetIndex` by one (insert after `targetIndex`),
                    // while 'else' branch will cover (2) (insert at `targetIndex`).
                    lowBoundary = ++targetIndex;
                }
                else
                {
                    highBoundary = targetIndex - 1;
                }
            }

            return targetIndex;
        }
    }
}
