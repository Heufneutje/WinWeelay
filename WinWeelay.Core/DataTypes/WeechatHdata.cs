using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WinWeelay.Core
{
    /// <summary>
    /// Hdata representation of a relay object.
    /// </summary>
    public class WeechatHdata : WeechatRelayObject, IEnumerable<WeechatHdataEntry>
    {
        private readonly List<WeechatHdataEntry> _items;

        /// <summary>
        /// Collection of paths for all objects present in the Hdata object.
        /// </summary>
        public string[] PathList { get; set; }

        /// <summary>
        /// The number of elements in the Hdata object.
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Create an empty Hdata object.
        /// </summary>
        public WeechatHdata()
        {
            Type = WeechatType.Hdata;
            _items = new List<WeechatHdataEntry>();
        }

        /// <summary>
        /// Add a new object to the Hdata object.
        /// </summary>
        /// <param name="hde">The object to add.</param>
        public void AddItem(WeechatHdataEntry hde)
        {
            _items.Add(hde);
        }

        /// <summary>
        /// Request an object from the Hdata structure at a given index.
        /// </summary>
        /// <param name="index">The given index.</param>
        /// <returns>The object at the given index.</returns>
        public WeechatHdataEntry this[int index] => _items[index];

        /// <summary>
        /// IEnumerable implementation.
        /// </summary>
        /// <returns>Enumerator for looping.</returns>
        public IEnumerator<WeechatHdataEntry> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Checker whether a given key is present in the keys of the hdata entries.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Whether or not the key is present iin the keys of the hdata entries</returns>
        public bool ContainsKey(string key)
        {
            return _items.Any(x => x.DataContainsKey(key));
        }
    }
}
