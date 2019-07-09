using System;
using System.IO;
using Newtonsoft.Json;

namespace WinWeelay.Configuration
{
    public static class ConfigurationHelper
    {
        private static readonly string _configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinWeelay", "config.json");

        public static RelayConfiguration LoadConfiguration()
        {
            RelayConfiguration relayConfiguration;
            if (File.Exists(_configPath))
                relayConfiguration = JsonConvert.DeserializeObject<RelayConfiguration>(File.ReadAllText(_configPath));
            else
                relayConfiguration = new RelayConfiguration();

            relayConfiguration.StartTrackingChanges();
            return relayConfiguration;
        }

        public static void SaveConfiguration(RelayConfiguration relayConfiguration)
        {
            JsonSerializer serializer = new JsonSerializer { Formatting = Formatting.Indented };
            using (StreamWriter writer = File.CreateText(_configPath))
                serializer.Serialize(writer, relayConfiguration);
        }
    }
}
