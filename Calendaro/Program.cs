using Calendaro.CalendarServices;
using Calendaro.EventsTracking;
using Calendaro.Properties;
using Calendaro.Settings;
using Calendaro.Settings.Json;
using Calendaro.Storage;
using Calendaro.UI;

namespace Calendaro
{
    /// <summary>
    /// Defines application entry point.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Name of the default settings file.
        /// </summary>
        private const string SettingsFileName = "settings.json";

        /// <summary>
        /// Entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

            // TODO: Use IoC container for DI? Nah, that's for pussies!
            var storagePathProvider = new AppDataStoragePathProvider();
            var settingsProvider =
                new CachedCalendaroSettingsProvider(
                    new JsonCalendaroSettingsProvider(
                        Path.Join(storagePathProvider.GetStoragePath(), SettingsFileName)));
            var calendarServicesFactory =
                new CachedCalendarServicesFactory(
                    new KnownCalendarServicesFactory(storagePathProvider.GetStoragePath()));

            using (var eventsManager = new EventsManager(calendarServicesFactory))
            {
                using (var notificationForm =
                    new NotificationForm(
                        settingsProvider,
                        calendarServicesFactory,
                        eventsManager,
                        new SolidColorCalendarIconsProvider(3)))
                {
                    Application.Run(notificationForm);
                }
            }
        }

        /// <summary>
        /// Displays message box with an error message and terminates the application.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var message = Resources.FatalErrorMessage;

            if (e.ExceptionObject is Exception error)
            {
                message = error.Message;
            }

            MessageBox.Show(message, Resources.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(1);
        }
    }
}