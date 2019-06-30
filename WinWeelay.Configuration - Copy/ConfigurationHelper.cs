using System.IO;
using Newtonsoft.Json;

namespace WinWeelay.Configuration
{
    public class ConfigurationHelper
    {
        private const string ConfigPath = "config.json";

        public static RelayConfiguration LoadConfiguration()
        {
            RelayConfiguration relayConfiguration;
            if (File.Exists(ConfigPath))
                relayConfiguration = JsonConvert.DeserializeObject<RelayConfiguration>(File.ReadAllText(ConfigPath));
            else
                relayConfiguration = new RelayConfiguration();

            relayConfiguration.StartTrackingChanges();
            return relayConfiguration;
        }

        public static void SaveConfiguration(RelayConfiguration relayConfiguration)
        {
            JsonSerializer serializer = new JsonSerializer { Formatting = Formatting.Indented };
            using (StreamWriter writer = File.CreateText(ConfigPath))
                serializer.Serialize(writer, relayConfiguration);

            relayConfiguration.ResetTrackingChanges();
            relayConfiguration.StartTrackingChanges();
        }
    }
}
