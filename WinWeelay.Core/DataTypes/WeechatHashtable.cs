using System.Collections.Generic;
using System.Text;

namespace WinWeelay.Core
{
    /// <summary>
    /// Hashtable representation of a relay object.
    /// </summary>
    public class WeechatHashtable : WeechatRelayObject
    {
        private Dictionary<string, WeechatRelayObject> _dict = new Dictionary<string, WeechatRelayObject>();

        /// <summary>
        /// Add a new item to the hashtable.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void Add(WeechatRelayObject key, WeechatRelayObject value)
        {
            _dict.Add(key.ToString(), value);
        }

        /// <summary>
        /// Retrieves a relay object from the hashtable.
        /// </summary>
        /// <param name="key">The key of the element to retrieve.</param>
        /// <returns>The value for the given key.</returns>
        public WeechatRelayObject this[string key]
        {
            get
            {
                return _dict[key];
            }
        }

        /// <summary>
        /// Checker whether a given key is present in the hashtabel.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Whether or not the key is present in the hashtable.</returns>
        public bool ContainsKey(string key)
        {
            return _dict.ContainsKey(key);
        }

        /// <summary>
        /// Override for debug purposes.
        /// </summary>
        /// <returns>All values in the hashtable.</returns>
        public override string ToString()
        {
            StringBuilder map = new StringBuilder();
            foreach (KeyValuePair<string, WeechatRelayObject> pair in _dict)
            {
                map.Append(pair.Key);
                map.Append(" -> ");
                map.Append(pair.Value);
                map.Append(", ");
            }
            return map.ToString();
        }
    }
}
