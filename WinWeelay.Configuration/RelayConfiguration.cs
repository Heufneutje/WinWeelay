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
        public bool SyncReadMessages { get; set; }
        
        [JsonIgnore]
        public string ConnectionAddress
        {
            get
            {
                return $"{Hostname}:{Port}";
            }
        }

        [JsonIgnore]
        public IEnumerable<ConnectionTypeWrapper> ConnectionTypes
        {
            get
            {
                return ConnectionTypeWrapper.GetTypes();
            }
        }

        public RelayConfiguration()
        {
            BacklogSize = 256;
            Port = 9001;
        }
    }
}
