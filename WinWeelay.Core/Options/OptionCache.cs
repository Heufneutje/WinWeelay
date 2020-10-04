using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using WinWeelay.Configuration;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    /// <summary>
    /// Cache for color options.
    /// </summary>
    [Serializable]
    public class OptionCache
    {
        private static readonly string _optionCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinWeelay", "optioncache.json");

        /// <summary>
        /// The date the options were retrieved.
        /// </summary>
        public DateTime CacheDate { get; set; }

        /// <summary>
        /// The cached color options.
        /// </summary>
        public List<RelayOption> Options { get; set; }

        /// <summary>
        /// Are there currently options in the cache?
        /// </summary>
        [JsonIgnore]
        public bool HasOptions => Options.Any();

        /// <summary>
        /// Create an empty cache.
        /// </summary>
        public OptionCache()
        {
            Options = new List<RelayOption>();
        }

        /// <summary>
        /// Create a new cache from a collection of color options.
        /// </summary>
        /// <param name="options">The given collection of color options.</param>
        public OptionCache(List<RelayOption> options)
        {
            Options = options;
        }

        /// <summary>
        /// Load the option cache if it has not yet expired.
        /// </summary>
        /// <param name="relayConfiguration">The configuration which specifies how long options should be cached.</param>
        /// <returns>The cached options or an empty cache if it has expired.</returns>
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

        /// <summary>
        /// Save the current cache.
        /// </summary>
        /// <param name="relayConfiguration">The configuration which specifies how long options should be cached.</param>
        public void SaveOptionCache(RelayConfiguration relayConfiguration)
        {
            if (!relayConfiguration.UseOptionCache || CacheDate.AddDays(relayConfiguration.OptionCacheDays) > DateTime.Now)
                return;

            CacheDate = DateTime.Now;
            JsonUtils.SaveSerializedObject(this, _optionCachePath);
        }
    }
}
