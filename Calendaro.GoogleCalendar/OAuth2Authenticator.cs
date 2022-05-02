using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;

namespace Calendaro.GoogleCalendar
{
    /// <summary>
    /// Implements OAuth2 authentication flow for the Google API.
    /// </summary>
    /// <remarks>
    /// OAuth2 protocol is not ideal, as there is no way to claim which account you want to authenticate.
    /// The only way to influence it, is through `login_hint`, but that merely pre-populates the username
    /// text field, while still alowing user to change it to whatever they want. In case of multiple sign-in,
    /// we need to have separate secure storage location for each token, and that is controlled by the `user`
    /// argument passed into `GoogleWebAuthorizationBroker.AuthorizeAsync`. We use <paramref name="accountId"/>
    /// for both, `login_hint` and user secrets store, but an actual logged-in account may end up being different.
    /// Ideally we would want both to match, so that it's clear which account in the UI corresponds to which Google
    /// account, but we cannot alter storage folder after we call `AuthorizeAsync`. The end result of this is that
    /// when token expires (or gets deleted) and you are asked to login again, you will have an <paramref name="accountId"/>
    /// shown in the username textbox on the authenticatio form, which may not match, if you previously
    /// logged-in to different Google account under this internal `accountId`.
    /// Bottom line is - out internal `accountId` is merely an alias within the application. When adding Google accounts
    /// to the application, try to keep them the same as your email address, so that you can clearly identify which
    /// Google account you need to login to, if prompted later with miltiple sign-in.
    /// </remarks>
    public sealed class OAuth2Authenticator : IAuthenticator
    {
        /// <summary>
        /// Singleton instance of the OAuth2 authenticator.
        /// </summary>
        private readonly string accountId;

        /// <summary>
        /// Path to the folder where authentication token file should be stored.
        /// </summary>
        private readonly string tokenStoragePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Authenticator"/> class
        /// with the provided account identifier and local token storage path.
        /// </summary>
        /// <param name="accountId">An identifier of the account to authenticate. Usually this will be your email address.</param>
        /// <param name="tokenStoragePath">Path to the folder where authentication token file should be stored.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public OAuth2Authenticator(string accountId, string tokenStoragePath)
        {
            this.accountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
            this.tokenStoragePath =
                Path.GetFullPath(tokenStoragePath ?? throw new ArgumentNullException(nameof(tokenStoragePath)));
        }

        /// <summary>
        /// Performs interactive authentication flow, if there is no user token available
        /// in the secure storage for the given account.
        /// </summary>
        /// <param name="cancellation">Cancellation token to stop authentication.</param>
        /// <returns>A <see cref="Task{TResult}"/>, which when completed returns API access token.</returns>
        public async Task<UserCredential> Authenticate(CancellationToken cancellation)
        {
            var authRequest =
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    // While this is called "secrets" in Google's terminology, this is not really a secret.
                    // This merely identifies your app to the identity provider, so that they can prompt
                    // user for consent stating your app as requestor.
                    ClientSecrets =
                        new ClientSecrets
                        {
                            ClientId = "<id>",
                            ClientSecret = "<secret>",
                        },
                    LoginHint = accountId
                };

            var accessScopes =
                new List<string>
                {
                    CalendarService.Scope.CalendarReadonly,
                    CalendarService.Scope.CalendarEventsReadonly,
                };

            return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                authRequest,
                accessScopes,
                accountId,
                cancellation,
                new FileDataStore(tokenStoragePath, true));
        }
    }
}
