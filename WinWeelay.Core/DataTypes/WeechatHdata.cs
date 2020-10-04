using System;
using System.Collections;
using System.Collections.Generic;

namespace WinWeelay.Core
{
    /// <summary>
    /// Hdata representation of a relay object.
    /// </summary>
    public class WeechatHdata : WeechatRelayObject, IEnumerable<WeechatHdataEntry>
    {
        private List<WeechatHdataEntry> _items;
        
        /// <summary>
        /// Collection of all keys present in the Hdata object.
        /// </summary>
        public string[] KeyList { get; set; }

        /// <summary>
        /// Collection of paths for all objects present in the Hdata object.
        /// </summary>
        public string[] PathList { get; set; }

        /// <summary>
        /// Collection of types for each object in the Hdata object.
        /// </summary>
        public WeechatType[] TypeList { get; set; }

        /// <summary>
        /// The number of elements in the Hdata object.
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Create an empty Hdata object.
        /// </summary>
        public WeechatHdata()
        {
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
        /// Initialize the collection of keys. Each key is formatted as a key name and a type separated by a colon.
        /// </summary>
        /// <param name="keys">The collection of keys</param>
        public void SetKeys(string[] keys)
        {
            KeyList = new string[keys.Length];
            TypeList = new WeechatType[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                string[] kt = keys[i].Split(new char[] { ':' });
                KeyList[i] = kt[0];
                TypeList[i] = (WeechatType)Enum.Parse(typeof(WeechatType), kt[1].ToUpper());
            }
        }

        /// <summary>
        /// Request an object from the Hdata structure at a given index.
        /// </summary>
        /// <param name="index">The given index.</param>
        /// <returns>The object at the given index.</returns>
        public WeechatHdataEntry this[int index] => _items[index];

        /// <summary>
        /// Override for debug purposes.
        /// </summary>
        /// <returns>All values in the Hdata object.</returns>
        public override string ToString()
        {
            string s = $"[WHdata]{Environment.NewLine}  path=";
            if (PathList == null)
                s += "null";
            else
                foreach (string p in PathList)
                    s += p + "/";

            s += Environment.NewLine;
            foreach (WeechatHdataEntry hde in _items)
                s += hde.ToString(2) + Environment.NewLine;
            return s;
        }

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
    }
}
