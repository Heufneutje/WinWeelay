using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WinWeelay.Utils;

namespace WinWeelay.Configuration
{
    [Serializable]
    public class RelayConfiguration : BaseChangeTrackable
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string RelayPassword { get; set; }
        public int BacklogSize { get; set; }
        public bool AutoConnect { get; set; }
        public RelayConnectionType ConnectionType { get; set; }
        public string WebSocketPath { get; set; }
        public bool SyncReadMessages { get; set; }
        public int HistorySize { get; set; }
        public int FontSize { get; set; }
        public string FontFamily { get; set; }
        public double WindowWidth { get; set; }
        public double WindowHeight { get; set; }
        public string TimestampFormat { get; set; }
        public string Theme { get; set; }
        public bool AutoCheckUpdates { get; set; }
        public bool IsSpellCheckEnabled { get; set; }
        public bool IsMessageFormattingEnabled { get; set; }
        public bool NotificationsEnabled { get; set; }
        public BufferViewType BufferViewType { get; set; }
        public AccentColor AccentColor { get; set; }
        public bool IsToolbarVisible { get; set; }
        public bool IsStatusBarVisible { get; set; }
        
        [JsonIgnore]
        [ChangeTrackingIgnore]
        public string ConnectionAddress
        {
            get
            {
                return $"{Hostname}:{Port}";
            }
        }

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
        }

        public override void StartTrackingChanges()
        {
            base.StartTrackingChanges();
            AccentColor.StartTrackingChanges();
        }

        public void NotifyViewPropertiesChanged()
        {
            NotifyPropertyChanged(nameof(IsToolbarVisible));
            NotifyPropertyChanged(nameof(IsStatusBarVisible));
        }
    }
}
