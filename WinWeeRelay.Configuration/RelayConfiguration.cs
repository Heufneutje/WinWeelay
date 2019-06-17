using System;
using Newtonsoft.Json;
using WinWeeRelay.Utils;

namespace WinWeeRelay.Configuration
{
    [Serializable]
    public class RelayConfiguration
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string RelayPassword { get; set; }
        public int BacklogSize { get; set; }

        public RelayConfiguration()
        {
            BacklogSize = -1;
        }

        [JsonIgnore]
        public string DecryptedRelayPassword
        {
            get
            {
                if (string.IsNullOrEmpty(RelayPassword))
                    return null;

                return Cipher.Decrypt(RelayPassword);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    RelayPassword = Cipher.Encrypt(value);
            }
        }
    }
}
