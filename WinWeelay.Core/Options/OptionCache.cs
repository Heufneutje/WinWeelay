using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using WinWeelay.Configuration;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    [Serializable]
    public class OptionCache
    {
        private static readonly string _optionCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinWeelay", "optioncache.json");

        public DateTime CacheDate { get; set; }
        public List<RelayOption> Options { get; set; }

        [JsonIgnore]
        public bool HasOptions => Options.Any();

        public OptionCache()
        {
            Options = new List<RelayOption>();
        }

        public OptionCache(List<RelayOption> options)
        {
            Options = options;
        }

        public static OptionCache Load(RelayConfiguration relayConfiguration)
        {
            if (relayConfiguration == null || !relayConfiguration.UseOptionCache)
                return new OptionCache();

            OptionCache optionCache;
            if (!File.Exists(_optionCachePath))
                return new OptionCache();

            optionCache = JsonUtils.DeserializeObject<OptionCache>(File.ReadAllText(_optionCachePath));
            if (optionCache.CacheDate.AddDays(relayConfiguration.OptionCacheDays) < DateTime.Now)
                return new OptionCache();

            return optionCache;
        }

        public void SaveOptionCache(RelayConfiguration relayConfiguration)
        {
            if (!relayConfiguration.UseOptionCache || CacheDate.AddDays(relayConfiguration.OptionCacheDays) > DateTime.Now)
                return;

            CacheDate = DateTime.Now;
            JsonUtils.SaveSerializedObject(this, _optionCachePath);
        }
    }
}
