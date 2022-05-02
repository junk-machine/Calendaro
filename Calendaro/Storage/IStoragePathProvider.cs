namespace Calendaro.Storage
{
    /// <summary>
    /// Determines path to the local persistent storage, which can be used to store
    /// settings and access tokens.
    /// </summary>
    internal interface IStoragePathProvider
    {
        /// <summary>
        /// Gets the path to the local persistent storage.
        /// </summary>
        /// <returns>Local persistent storage path.</returns>
        string GetStoragePath();
    }
}
