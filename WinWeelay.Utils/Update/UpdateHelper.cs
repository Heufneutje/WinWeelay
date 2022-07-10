using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WinWeelay.Utils
{
    /// <summary>
    /// Utility class handling application updates.
    /// </summary>
    public class UpdateHelper : IDisposable
    {
        private const string _GITHUB_BASE_URL = "https://api.github.com";

        /// <summary>
        /// Temp path of the downloaded installer file.
        /// </summary>
        public string InstallerFilePath { get; private set; } = Path.Combine(Path.GetTempPath(), "WinWeelaySetup.exe");

        private HttpClient _client;

        /// <summary>
        /// Delegate for checking the download progress.
        /// </summary>
        public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded, int? progressPercentage);

        /// <summary>
        /// Event for checking the download progress.
        /// </summary>
        public event ProgressChangedHandler ProgressChanged;

        /// <summary>
        /// Event signaling that the download is complete.
        /// </summary>
        public event AsyncCompletedEventHandler DownloadCompleted;

        /// <summary>
        /// Create a new instance for handling updates.
        /// </summary>
        public UpdateHelper()
        {
            _client = new HttpClient();
            SetHeaders(_client);
        }

        /// <summary>
        /// Check whether a new release exists.
        /// </summary>
        /// <returns>Whether a new release exists.</returns>
        public async Task<UpdateCheckResult> CheckForUpdateAsync()
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync($"{_GITHUB_BASE_URL}/repos/heufneutje/winweelay/releases");  
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return new UpdateCheckResult(UpdateResultType.Error, $"An error occurred while downloading the update file.{Environment.NewLine}{content ?? response.ReasonPhrase}", "Error", null);
                
                IEnumerable<GitHubRelease> releases = JsonUtils.DeserializeObject<List<GitHubRelease>>(content).Where(x => !x.Prerelease);
                if (!releases.Any())
                    return null;

                GitHubRelease latestVersion = releases.First();
                string latestVersionStr = latestVersion.TagName.Substring(1);
                FileVersionInfo fvi = GetCurrentVersion();
                string currentVersion = string.Join(".", new int[3] { fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart });
                if (VersionsEqual(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, latestVersionStr))
                    return new UpdateCheckResult(UpdateResultType.NoUpdateAvailable, $"Current version: {currentVersion}{Environment.NewLine}{Environment.NewLine}You're running the latest version of WinWeelay.", "No update available", null);

                StringBuilder updateTextBuilder = new StringBuilder();
                updateTextBuilder.AppendLine($"Current version: {currentVersion}");
                updateTextBuilder.AppendLine($"Latest version: {latestVersionStr}");
                updateTextBuilder.AppendLine();
                updateTextBuilder.AppendLine($"Changelog:");
                updateTextBuilder.AppendLine(latestVersion.Body);
                updateTextBuilder.AppendLine();
                updateTextBuilder.AppendLine("Would you like to download this version now?");

                return new UpdateCheckResult(UpdateResultType.UpdateAvailable, updateTextBuilder.ToString(), "Update available", latestVersion.Assets.First().BrowserDownloadUrl);
            }
            catch (Exception ex)
            {
                return new UpdateCheckResult(UpdateResultType.Error, $"An error occurred while downloading the update file.{Environment.NewLine}{ex.Message}", "Error", null);
            }
        }

        /// <summary>
        /// Download the release from the given URL.
        /// </summary>
        /// <param name="downloadUrl">The URL to download the update from.</param>
        public async Task DownloadUpdateAsync(string downloadUrl)
        {
            string filePath = InstallerFilePath;
            if (File.Exists(filePath))
                File.Delete(filePath);

            using (HttpResponseMessage response = await _client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                await DownloadFileFromHttpResponseMessageAsync(response);
        }

        /// <summary>
        /// Get the directory the application is currently running from.
        /// </summary>
        /// <returns>The directory the application is currently running from.</returns>
        public string GetApplicationDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        /// <summary>
        /// Get the current version of the application.
        /// </summary>
        /// <returns>A version info object representing the current version.</returns>
        public static FileVersionInfo GetCurrentVersion()
        {
            return FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
        }

        private bool VersionsEqual(int curMajor, int curMinor, int curPatch, string latest)
        {
            string[] parts = latest.Split('.');
            if (Convert.ToInt32(parts[0]) > curMajor)
                return false;

            if (Convert.ToInt32(parts[1]) > curMinor)
                return false;

            if (Convert.ToInt32(parts[2]) > curPatch)
                return false;

            return true;
        }

        private void SetHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
        }

        private async Task DownloadFileFromHttpResponseMessageAsync(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            long? totalBytes = response.Content.Headers.ContentLength;

            using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                await ProcessContentStreamAsync(totalBytes, contentStream);
        }

        private async Task ProcessContentStreamAsync(long? totalDownloadSize, Stream contentStream)
        {
            Exception exception = null;

            try
            {
                long totalBytesRead = 0L;
                long readCount = 0L;
                byte[] buffer = new byte[8192];
                bool isMoreToRead = true;

                using (FileStream fileStream = new FileStream(InstallerFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    do
                    {
                        int bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            isMoreToRead = false;
                            TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                            continue;
                        }

                        await fileStream.WriteAsync(buffer, 0, bytesRead);

                        totalBytesRead += bytesRead;
                        readCount += 1;

                        if (readCount % 100 == 0)
                            TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                    }
                    while (isMoreToRead);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            DownloadCompleted?.Invoke(this, new AsyncCompletedEventArgs(exception, false, null));
        }

        private void TriggerProgressChanged(long? totalDownloadSize, long totalBytesRead)
        {
            if (ProgressChanged == null)
                return;

            int? progressPercentage = null;
            if (totalDownloadSize.HasValue)
                progressPercentage = Convert.ToInt32((Math.Round((double)totalBytesRead / totalDownloadSize.Value * 100, 2)));

            ProgressChanged(totalDownloadSize, totalBytesRead, progressPercentage);
        }

        #region IDisposable Support

        private bool _disposedValue = false;

        /// <summary>
        /// IDisposable implementation.
        /// </summary>
        /// <param name="disposing">IDisposable implementation.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                    _client.Dispose();

                _disposedValue = true;
            }
        }

        /// <summary>
        /// IDisposable implementation.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}
