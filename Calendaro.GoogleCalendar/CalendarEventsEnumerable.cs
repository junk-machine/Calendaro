using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace Calendaro.GoogleCalendar
{
    /// <summary>
    /// Enumerable for the paged events data.
    /// </summary>
    /// <remarks>
    /// This enumerable wraps around the Google Calendar API request and retrieves paged data
    /// as if it was a single continuous collection.
    /// </remarks>
    internal class CalendarEventsEnumerable : IAsyncEnumerable<Event>
    {
        /// <summary>
        /// Events request that is used to retrieve paged data.
        /// </summary>
        private readonly EventsResource.ListRequest initialRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneTimeAsyncEnumerable"/> class
        /// with the provided initial events request configuration.
        /// </summary>
        /// <param name="initialRequest">Initial events request configuration.</param>
        public CalendarEventsEnumerable(EventsResource.ListRequest initialRequest)
        {
            // TODO: Clone request so that we can enumerate collection in parallel?
            //       Otherwise `PageToken` property will be modified in-place by the enumerator.
            this.initialRequest = initialRequest;
        }

        /// <inheritdoc/>
        public IAsyncEnumerator<Event> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new CalendarEventsEnumerator(initialRequest, cancellationToken);
        }

        /// <summary>
        /// Enumerator for the paged events data.
        /// </summary>
        private sealed class CalendarEventsEnumerator : IAsyncEnumerator<Event>
        {
            /// <summary>
            /// Events request that is used to retrieve paged data.
            /// </summary>
            private readonly EventsResource.ListRequest request;

            /// <summary>
            /// Cancellation token to stop enumeration.
            /// </summary>
            private readonly CancellationToken cancellation;

            /// <summary>
            /// Response that holds last requested page of events data.
            /// </summary>
            private Events? lastResponse;

            /// <summary>
            /// Current item index within the active page.
            /// </summary>
            private int currentIndex;

            /// <summary>
            /// Initializes a new instance of the <see cref="CalendarEventsEnumerator"/> class
            /// with the provided initial events request configuration.
            /// </summary>
            /// <param name="initialRequest">Initial events request configuration.</param>
            /// <param name="cancellation">Cancellation token to stop enumeration.</param>
            public CalendarEventsEnumerator(EventsResource.ListRequest initialRequest, CancellationToken cancellation)
            {
                request = initialRequest;
                this.cancellation = cancellation;
            }

            /// <summary>
            /// Gets the current event item.
            /// </summary>
            public Event Current
            {
                get
                {
                    if (lastResponse == null || currentIndex >= lastResponse.Items.Count)
                    {
                        // Enumerator was not initialized yet or all the data was already exhausted
                        throw new InvalidOperationException();
                    }

                    return lastResponse.Items[currentIndex];
                }
            }

            /// <inheritdoc/>
            public ValueTask DisposeAsync()
            {
                // There is nothing to dispose as Google API requests and reponses are stateless
                return default;
            }

            /// <inheritdoc/>
            public async ValueTask<bool> MoveNextAsync()
            {
                // If we did not fetch the next page yet or we exhausted the active one
                while (lastResponse == null
                    || currentIndex >= lastResponse.Items.Count - 1)
                {
                    // If it's not the first page, then we need to update the request
                    if (lastResponse != null)
                    {
                        if (lastResponse.NextPageToken != null)
                        {
                            // If there is next page available, then we fetch it
                            request.PageToken = lastResponse.NextPageToken;
                        }
                        else
                        {
                            // Otherwise there is simply no more data
                            // Setup request to query just the change feed using the sync token
                            request.SyncToken = lastResponse.NextSyncToken;
                            request.PageToken = null;
                            lastResponse = null;
                            return false;
                        }
                    }

                    lastResponse = await request.ExecuteAsync(cancellation);
                    currentIndex = -1;
                }

                ++currentIndex;
                return true;
            }
        }
    }
}
