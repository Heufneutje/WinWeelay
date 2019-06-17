using System.IO;
using Newtonsoft.Json;

namespace WinWeeRelay.Configuration
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
            LoadDefaults(relayConfiguration);

            return relayConfiguration;
        }

        public static void SaveConfiguration(RelayConfiguration relayConfiguration)
        {
            JsonSerializer serializer = new JsonSerializer { Formatting = Formatting.Indented };
            using (StreamWriter writer = File.CreateText(ConfigPath))
                serializer.Serialize(writer, relayConfiguration);
        }

        private static void LoadDefaults(RelayConfiguration relayConfiguration)
        {
            if (relayConfiguration.BacklogSize == -1)
                relayConfiguration.BacklogSize = 100;
        }
    }
}
