using System.Collections.Generic;
using Newtonsoft.Json;

namespace WinWeelay.Utils
{
    /// <summary>
    /// Class for deserializing GitHub release responses.
    /// </summary>
    public class GitHubRelease
    {
        /// <summary>
        /// The version of the release.
        /// </summary>
        [JsonProperty(PropertyName = "tag_name")]
        public string TagName { get; set; }

        /// <summary>
        /// Whether the release is a prerelease.
        /// </summary>
        [JsonProperty(PropertyName = "prerelease")]
        public bool Prerelease { get; set; }

        /// <summary>
        /// The changelog of the release.
        /// </summary>
        [JsonProperty(PropertyName = "body")]
        public string Body { get; set; }

        /// <summary>
        /// The downloadable assets attached to the release.
        /// </summary>
        [JsonProperty(PropertyName = "assets")]
        public List<GitHubReleaseAsset> Assets { get; set; }
    }
}
