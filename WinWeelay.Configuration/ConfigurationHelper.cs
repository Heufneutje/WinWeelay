using System;
using System.IO;
using WinWeelay.Utils;

namespace WinWeelay.Configuration
{
    public static class ConfigurationHelper
    {
        private static readonly string _configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinWeelay", "config.json");

        public static RelayConfiguration LoadConfiguration()
        {
            RelayConfiguration relayConfiguration;
            if (File.Exists(_configPath))
                relayConfiguration = JsonUtils.DeserializeObject<RelayConfiguration>(File.ReadAllText(_configPath));
            else
                relayConfiguration = new RelayConfiguration();

            relayConfiguration.StartTrackingChanges();
            return relayConfiguration;
        }

        public static void SaveConfiguration(RelayConfiguration relayConfiguration)
        {
            JsonUtils.SaveSerializedObject(relayConfiguration, _configPath);
        }
    }
}
