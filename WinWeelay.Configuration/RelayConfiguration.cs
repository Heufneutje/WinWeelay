using System;
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

        public RelayConfiguration()
        {
            BacklogSize = -1;
        }
    }
}
