using Google.Apis.Auth.OAuth2;

namespace Calendaro.GoogleCalendar
{
    /// <summary>
    /// Defines authentication flow for the Google API.
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Retrieves credentials (access token) to access Google API for the given user account.
        /// </summary>
        /// <param name="cancellation">Cancellation token to stop authentication.</param>
        /// <returns>A <see cref="Task{TResult}"/>, which when completed returns API access token.</returns>
        Task<UserCredential> Authenticate(CancellationToken cancellation);
    }
}
