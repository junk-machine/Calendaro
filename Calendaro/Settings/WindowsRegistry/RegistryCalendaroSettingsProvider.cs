using Microsoft.Win32;

namespace Calendaro.Settings.WindowsRegistry
{
    /// <summary>
    /// Application settings provider that persists configuration in the Windows registry.
    /// </summary>
    /// <remarks>
    /// Current implementation does not persist anything aside from the auto-start option.
    /// This allows other settings providers to inherit from it and have fully functional
    /// auto-start setting.
    /// </remarks>
    internal class RegistryCalendaroSettingsProvider : ICalendaroSettingsProvider
    {
        /// <summary>
        /// Registry key that holds list of application that should be started automatically.
        /// </summary>
        private const string AutoStartRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>
        /// Default name of the registry value for the application.
        /// </summary>
        private const string DefaultAppRunValueName = "Calendaro .NET";

        /// <summary>
        /// Full path to the application executable file.
        /// </summary>
        private static readonly string ApplicationPath =
            Environment.ProcessPath ?? Environment.GetCommandLineArgs()[0];

        /// <summary>
        /// Reads registry key to determine whether application is configured to start automatically.
        /// </summary>
        /// <param name="cancellation">Cancellation token to stop the loading operation.</param>
        /// <returns>A <see cref="Task{TResult}"/>, which when completed provides application settings.</returns>
        public virtual Task<CalendaroSettings> LoadSettingsAsync(CancellationToken cancellation)
        {
            return Task.FromResult(
                new CalendaroSettings
                {
                    AutoStart = FindAutoStartValueName() is not null,
                });
        }

        /// <summary>
        /// Creates or removes a `Run` registry value based on the <see cref="CalendaroSettings.AutoStart"/>
        /// property.
        /// </summary>
        /// <param name="settings">New application settings to persist.</param>
        /// <param name="cancellation">Cancellation token to stop the save operation.</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous save operation.</returns>
        public virtual Task SaveSettingsAsync(CalendaroSettings settings, CancellationToken cancellation)
        {
            var existingValueName = FindAutoStartValueName();

            if (existingValueName is null == settings.AutoStart)
            {
                // If new configuration does not match the registry
                using var runRegistryKey =
                    Registry.CurrentUser.OpenSubKey(AutoStartRegistryKey, true);

                if (runRegistryKey != null)
                {
                    // We could check `settings.AutoStart`, but C# nullable types DFA is not
                    // smart enough to figure out that `existingValueName` is not going to be null,
                    // if we reach "else" clause here basded on the `settings.AutoStart` condition.
                    // We are obviously smarter, we know that nullability of `existingValueName` is
                    // aligned with `settings.AutoStart`, so we check yet again here whether value
                    // already exists in the registry or not and act based on that. This keeps C#
                    // happy in the "else" branch, where we access `existingValueName` variable.
                    if (existingValueName is null)
                    {
                        // Add new registry value, so that application starts automatically
                        runRegistryKey.SetValue(DefaultAppRunValueName, ApplicationPath);
                    }
                    else
                    {
                        // Remove exiting registry value, so that application does not start automatically
                        runRegistryKey.DeleteValue(existingValueName, false);
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Searches for the application executable path under auto-start registry key
        /// and returns the name of the mathed value.
        /// If there is no value currently, then null is returned.
        /// </summary>
        /// <returns>Name of the auto-start value for the application or null.</returns>
        private static string? FindAutoStartValueName()
        {
            using var runRegistryKey =
                Registry.CurrentUser.OpenSubKey(AutoStartRegistryKey, false);

            if (runRegistryKey != null)
            {
                foreach (var valueName in runRegistryKey.GetValueNames())
                {
                    // We don't want to rely on just the value name. Instead we scan through
                    // all values and check if there is a value with our executable path.
                    if (runRegistryKey.GetValue(valueName) is string stringValue
                        && string.Equals(stringValue, ApplicationPath, StringComparison.OrdinalIgnoreCase))
                    {
                        return valueName;
                    }
                }
            }

            return null;
        }
    }
}
