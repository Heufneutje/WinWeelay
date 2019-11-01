namespace WinWeelay.Utils
{
    /// <summary>
    /// Enum to represent the update state.
    /// </summary>
    public enum UpdateResultType
    {
        /// <summary>
        /// An update is available to download.
        /// </summary>
        UpdateAvailable = 1,

        /// <summary>
        /// There is no update available to download. The current version is already the latest.
        /// </summary>
        NoUpdateAvailable = 2,

        /// <summary>
        /// An error occurred while checking for updates.
        /// </summary>
        Error = 3
    }

    /// <summary>
    /// The result of the update check on GitHub.
    /// </summary>
    public class UpdateCheckResult
    {
        /// <summary>
        /// The update status.
        /// </summary>
        public UpdateResultType ResultType { get; private set; }

        /// <summary>
        /// The changelog of the release.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The title of the update (version name/number).
        /// </summary>
        public string MessageTitle { get; private set; }

        /// <summary>
        /// The URL to download the update from.
        /// </summary>
        public string DownloadUrl { get; private set; }

        /// <summary>
        /// Create a new update result.
        /// </summary>
        /// <param name="resultType">The update status.</param>
        /// <param name="message">The changelog of the release.</param>
        /// <param name="messageTitle">The title of the update (version name/number).</param>
        /// <param name="downloadUrl">The URL to download the update from.</param>
        public UpdateCheckResult(UpdateResultType resultType, string message, string messageTitle, string downloadUrl)
        {
            ResultType = resultType;
            Message = message;
            MessageTitle = messageTitle;
            DownloadUrl = downloadUrl;
        }
    }
}
