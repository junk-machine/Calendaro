namespace Calendaro.Storage
{
    /// <summary>
    /// Determines path to the local persistent storage under the user's AppData folder.
    /// </summary>
    internal class AppDataStoragePathProvider : IStoragePathProvider
    {
        /// <summary>
        /// Name of the application-specific folder under AppData.
        /// </summary>
        private const string ApplicationFolderName = "Calendaro-NET";

        /// <summary>
        /// Gets the local persistent storage path under the user's AppData folder.
        /// </summary>
        /// <returns></returns>
        public string GetStoragePath() =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ApplicationFolderName);
    }
}
