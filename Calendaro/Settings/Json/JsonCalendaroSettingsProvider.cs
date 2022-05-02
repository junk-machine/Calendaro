using Calendaro.Settings.WindowsRegistry;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Text;

namespace Calendaro.Settings.Json
{
    /// <summary>
    /// Application settings provider that persists configuration in the JSON file.
    /// </summary>
    /// <remarks>
    /// We inherit from the <see cref="RegistryCalendaroSettingsProvider"/> to store
    /// auto-start setting in the `Run` registry key. The rest of the settings will be
    /// stored in the JSON file.
    /// </remarks>
    internal class JsonCalendaroSettingsProvider : RegistryCalendaroSettingsProvider
    {
        /// <summary>
        /// Path to the configuration file.
        /// </summary>
        private readonly string filePath;

        /// <summary>
        /// JSON serializer used to store and retrieve the settings from file.
        /// </summary>
        private readonly JsonSerializer jsonSerializer =
            JsonSerializer.Create(
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new IgnoreAutoStartContractResolver(),
                });

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCalendaroSettingsProvider"/> class
        /// with the provided configuration file path.
        /// </summary>
        /// <param name="filePath">Path to the configuration file.</param>
        /// <exception cref="ArgumentNullException">One of the required arguments is not provided.</exception>
        public JsonCalendaroSettingsProvider(string filePath)
        {
            this.filePath =
                Path.GetFullPath(filePath ?? throw new ArgumentNullException(nameof(filePath)));
        }

        /// <summary>
        /// Deserializes application configuration from the JSON file.
        /// </summary>
        /// <param name="cancellation">Cancellation token to stop the loading operation.</param>
        /// <returns>A <see cref="Task{TResult}"/>, which when completed provides application settings.</returns>
        public override async Task<CalendaroSettings> LoadSettingsAsync(CancellationToken cancellation)
        {
            var settings = await base.LoadSettingsAsync(cancellation);

            // Populate additional settings from the JSON file
            try
            {
                using (var textReader = new StreamReader(filePath, Encoding.UTF8))
                {
                    jsonSerializer.Populate(textReader, settings);
                }
            }
            catch (IOException)
            {
                // No settings file exist or we don't have access.
                // Just start from scratch and let user configure and fail on save.
            }
            
            return settings;
        }

        /// <summary>
        /// Serializes application configuration to the JSON file.
        /// </summary>
        /// <param name="settings">New application settings to persist.</param>
        /// <param name="cancellation">Cancellation token to stop the save operation.</param>
        /// <returns>A <see cref="Task"/> that represents an asynchronous save operation.</returns>
        public override async Task SaveSettingsAsync(CalendaroSettings settings, CancellationToken cancellation)
        {
            // Ensure storage directory exists
            var storageFolderPath = Path.GetDirectoryName(filePath);
            if (storageFolderPath is not null)
            {
                // If it's not a root of the drive
                Directory.CreateDirectory(storageFolderPath);
            }

            // Serialize settings to the JSON file
            using (var textWriter = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                jsonSerializer.Serialize(textWriter, settings);
            }

            // Call base implementation to store `settings.AutoStart` property in the registry
            await base.SaveSettingsAsync(settings, cancellation);
        }

        /// <summary>
        /// JSON.NET contract resolver that ignores <see cref="CalendaroSettings.AutoStart"/> property.
        /// </summary>
        /// <remarks>
        /// The <see cref="CalendaroSettings.AutoStart"/> property will be stored in the appropriate
        /// registry key and excluded from the JSON configuration file.
        /// </remarks>
        private sealed class IgnoreAutoStartContractResolver : DefaultContractResolver
        {
            /// <inheritdoc/>
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var jsonProperty = base.CreateProperty(member, memberSerialization);

                if (member.DeclaringType == typeof(CalendaroSettings)
                    && member.Name == nameof(CalendaroSettings.AutoStart))
                {
                    jsonProperty.Ignored = true;
                }

                return jsonProperty;
            }
        }
    }
}
