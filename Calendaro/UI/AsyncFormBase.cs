namespace Calendaro.UI
{
    /// <summary>
    /// Base class for any form that performs asynchronous operations.
    /// </summary>
    /// <remarks>
    /// This class provides a cancellation token that is canceled once form is closed,
    /// so that any pending actions can be stoped.
    /// </remarks>
    internal class AsyncFormBase : Form
    {
        /// <summary>
        /// Cancellation token source that is set when the form is closed.
        /// </summary>
        private readonly CancellationTokenSource formClosedTokenSource = new();

        /// <summary>
        /// Cancellation token source that is signaled when the form is closed.
        /// </summary>
        protected CancellationToken FormClosedToken => formClosedTokenSource.Token;

        /// <summary>
        /// Cancels and disposes the cancellation token source that signals form closure.
        /// </summary>
        /// <param name="disposing">Flag that indicates whether token source should be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    formClosedTokenSource.Cancel();
                    formClosedTokenSource.Dispose();
                }
                catch
                {
                    // Dispose is best effort only
                }
            }

            base.Dispose(disposing);
        }
    }
}
