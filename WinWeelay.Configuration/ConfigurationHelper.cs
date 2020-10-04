using System;
using System.IO;
using WinWeelay.Utils;

namespace WinWeelay.Configuration
{
    /// <summary>
    /// Utility class for loading and saving the configuration file.
    /// </summary>
    public static class ConfigurationHelper
    {
        private static readonly string _configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinWeelay", "config.json");

        /// <summary>
        /// Load the configuration file from its standard location.
        /// </summary>
        /// <returns>The configuration contained in the file.</returns>
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

        /// <summary>
        /// Save the given configuration to the configuration file.
        /// </summary>
        /// <param name="relayConfiguration">The configuration to save.</param>
        public static void SaveConfiguration(RelayConfiguration relayConfiguration)
        {
            JsonUtils.SaveSerializedObject(relayConfiguration, _configPath);
        }
    }
}
