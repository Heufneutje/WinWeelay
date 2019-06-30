﻿using System;
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
        public bool SyncReadMessages { get; set; }
        public int HistorySize { get; set; }
        public int FontSize { get; set; }
        public string FontFamily { get; set; }
        public double WindowWidth { get; set; }
        public double WindowHeight { get; set; }
        
        [JsonIgnore]
        [ChangeTrackingIgnore]
        public string ConnectionAddress
        {
            get
            {
                return $"{Hostname}:{Port}";
            }
        }

        [JsonIgnore]
        [ChangeTrackingIgnore]
        public IEnumerable<ConnectionTypeWrapper> ConnectionTypes
        {
            get
            {
                return ConnectionTypeWrapper.GetTypes();
            }
        }

        public RelayConfiguration()
        {
            Port = 9001;
            BacklogSize = 100;
            HistorySize = 50;
            FontFamily = "Calibri";
            FontSize = 12;
            WindowWidth = 800;
            WindowHeight = 500;
        }

        public void NotifyThemeChanged()
        {
            NotifyPropertyChanged(nameof(FontSize));
            NotifyPropertyChanged(nameof(FontFamily));
        }
    }
}
