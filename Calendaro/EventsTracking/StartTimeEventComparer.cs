namespace Calendaro.EventsTracking
{
    /// <summary>
    /// Comparer for the <see cref="TrackedEvent"/> class that is based on the event start time.
    /// If start time is the same, then calendar identifier and event identifier are compared to
    /// produce stable order.
    /// </summary>
    internal class StartTimeEventComparer : Comparer<TrackedEvent>
    {
        /// <summary>
        /// Singleton instance of the comparer.
        /// </summary>
        public static readonly StartTimeEventComparer Instance = new();

        /// <inheritdoc/>
        public override int Compare(TrackedEvent? x, TrackedEvent? y)
        {
            if (x is not null && y is not null)
            {
                // Start by comparing the start time
                var difference =
                    Comparer<DateTime>.Default.Compare(x.Event.StartTimeUtc, y.Event.StartTimeUtc);

                if (difference == 0)
                {
                    // For events with the same start time - compare calendar identifiers
                    difference =
                        Comparer<string>.Default.Compare(x.Calendar.Id, y.Calendar.Id);
                }

                if (difference == 0)
                {
                    // For events with the same start time and calendar identifiers - compare event identifier
                    difference =
                        Comparer<string>.Default.Compare(x.Event.Id, y.Event.Id);
                }

                return difference;
            }

            return
                x is null && y is null ? 0
                    // Null is considered less than any value
                    : x is null ? -1 : 1;
        }
    }
}
