# Calendaro .NET

Calendaro is a Windows desktop application that provides simple notifications interface for the events in Google Calendar(s). The app is inspired by the Outlook calendar reminders.

## Build and run

The application is built using .NET 6. Once you clone the repository, open the solution file (`Calendaro.sln`) in [Visual Studio](https://aka.ms/vs).

In order for the applicaton to work you will need to provide client ID and secret in the `Calendaro.GoogleCalendar\OAuth2Authenticator.cs` to access Google APIs. Navigate to [Google Cloud Console](https://console.cloud.google.com/) and register your application to obtain the values.

Happy coding!