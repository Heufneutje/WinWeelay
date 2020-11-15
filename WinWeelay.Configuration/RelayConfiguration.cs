using System;
using System.Globalization;
using Newtonsoft.Json;
using WinWeelay.Utils;

namespace WinWeelay.Configuration
{
    /// <summary>
    /// Main configuration for the relay.
    /// </summary>
    [Serializable]
    public class RelayConfiguration : BaseChangeTrackable
    {
        /// <summary>
        /// Hostname where the WeeChat host is running.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Port that the WeeChat host is listening on.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Encrypted password to authenticate with WeeChat.
        /// </summary>
        public string RelayPassword { get; set; }

        /// <summary>
        /// The number of messages to load when opening a buffer window.
        /// </summary>
        public int BacklogSize { get; set; }

        /// <summary>
        /// Automatically connect on startup.
        /// </summary>
        public bool AutoConnect { get; set; }

        /// <summary>
        /// The type of relay connection.
        /// </summary>
        public RelayConnectionType ConnectionType { get; set; }

        /// <summary>
        /// The path to use when using a WebSocket connection type.
        /// </summary>
        public string WebSocketPath { get; set; }

        /// <summary>
        /// Sync read messages with WeeChat.
        /// </summary>
        public bool SyncReadMessages { get; set; }

        /// <summary>
        /// The number of sent messages to remember.
        /// </summary>
        public int HistorySize { get; set; }

        /// <summary>
        /// The font size to use in buffer windows.
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// The font family to use in buffer windows.
        /// </summary>
        public string FontFamily { get; set; }

        /// <summary>
        /// The width of the main window.
        /// </summary>
        public double WindowWidth { get; set; }

        /// <summary>
        /// The height of the main window.
        /// </summary>
        public double WindowHeight { get; set; }

        /// <summary>
        /// The timestamp format to use for buffer messages.
        /// </summary>
        public string TimestampFormat { get; set; }

        /// <summary>
        /// The theme for the application.
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// Automatically check for updates.
        /// </summary>
        public bool AutoCheckUpdates { get; set; }

        /// <summary>
        /// Check spelling in input box.
        /// </summary>
        public bool IsSpellCheckEnabled { get; set; }

        /// <summary>
        /// Show colors and formatting in buffers.
        /// </summary>
        public bool IsMessageFormattingEnabled { get; set; }

        /// <summary>
        /// Show toast notifications.
        /// </summary>
        public bool NotificationsEnabled { get; set; }

        /// <summary>
        /// Show notifications even when the applicable buffer is in focus.
        /// </summary>
        public bool NotificationsEnabledWithBufferFocus { get; set; }

        /// <summary>
        /// Type of buffer selection in the UI.
        /// </summary>
        public BufferViewType BufferViewType { get; set; }

        /// <summary>
        ///  Color values for the accent color in the UI.
        /// </summary>
        public AccentColor AccentColor { get; set; }

        /// <summary>
        /// Show the toolbar.
        /// </summary>
        public bool IsToolbarVisible { get; set; }

        /// <summary>
        /// Show the status bar.
        /// </summary>
        public bool IsStatusBarVisible { get; set; }

        /// <summary>
        /// Show the formatting toolbar.
        /// </summary>
        public bool IsFormattingToolbarVisible { get; set; }

        /// <summary>
        /// Cache color/formatting options.
        /// </summary>
        public bool UseOptionCache { get; set; }

        /// <summary>
        /// Days to store the color/formatting options.
        /// </summary>
        public int OptionCacheDays { get; set; }

        /// <summary>
        /// Language for the spell checker.
        /// </summary>
        public CultureInfo Language { get; set; }

        /// <summary>
        /// Automatically clear top messages from message buffers as new messages are added.
        /// </summary>
        public bool AutoShrinkBuffer { get; set; }

        /// <summary>
        /// Hostname/port to connect to.
        /// </summary>
        [JsonIgnore]
        [ChangeTrackingIgnore]
        public string ConnectionAddress => $"{Hostname}:{Port}";

        /// <summary>
        /// Create a new configuration.
        /// </summary>
        public RelayConfiguration()
        {
            Port = 9001;
            WebSocketPath = "weechat";
            BacklogSize = 100;
            HistorySize = 50;
            FontFamily = "Calibri";
            FontSize = 12;
            WindowWidth = 800;
            WindowHeight = 500;
            TimestampFormat = "HH:mm";
            Theme = Themes.Light;
            AutoCheckUpdates = true;
            IsSpellCheckEnabled = true;
            IsMessageFormattingEnabled = true;
            NotificationsEnabled = true;
            AccentColor = new AccentColor(33, 99, 255);
            IsToolbarVisible = true;
            IsStatusBarVisible = true;
            IsFormattingToolbarVisible = true;
            UseOptionCache = true;
            OptionCacheDays = 7;
            Language = CultureInfo.CurrentCulture;
            AutoShrinkBuffer = true;
        }

        /// <summary>
        /// Set the property current values as the orignal values and start keeping track of property changes.
        /// </summary>
        public override void StartTrackingChanges()
        {
            base.StartTrackingChanges();
            AccentColor.StartTrackingChanges();
        }

        /// <summary>
        /// Update the UI elements that are now shown/hidden.
        /// </summary>
        public void NotifyViewPropertiesChanged()
        {
            NotifyPropertyChanged(nameof(IsToolbarVisible));
            NotifyPropertyChanged(nameof(IsStatusBarVisible));
        }
    }
}
