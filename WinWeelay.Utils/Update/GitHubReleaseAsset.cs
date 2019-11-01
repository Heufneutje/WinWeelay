using Newtonsoft.Json;

namespace WinWeelay.Utils
{
    /// <summary>
    /// A downloadable asset attached to the release.
    /// </summary>
    public class GitHubReleaseAsset
    {
        /// <summary>
        /// URL to download this asset.
        /// </summary>
        [JsonProperty(PropertyName = "browser_download_url")]
        public string BrowserDownloadUrl { get; set; }
    }
}
