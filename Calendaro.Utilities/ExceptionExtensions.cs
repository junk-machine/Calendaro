using System.Text;

namespace Calendaro.Utilities
{
    /// <summary>
    /// Defines extension methods for the <see cref="Exception"/> class.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Combines messages from all <see cref="AggregateException"/> instances.
        /// </summary>
        /// <param name="error">An error to aggregate messages from.</param>
        /// <returns>Combined error message from all <see cref="AggregateException"/> instances.</returns>
        public static string GetAggregateMessage(this Exception error)
        {
            if (error is AggregateException aggregateError)
            {
                var combinedMessage = new StringBuilder();

                foreach (var innerError in aggregateError.InnerExceptions)
                {
                    if (combinedMessage.Length > 0)
                    {
                        combinedMessage.AppendLine();
                    }

                    combinedMessage.Append(innerError.GetAggregateMessage());
                }

                return combinedMessage.ToString();
            }

            return error.Message;
        }
    }
}
