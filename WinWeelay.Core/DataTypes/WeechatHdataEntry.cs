using System;
using System.Collections.Generic;
using System.Linq;

namespace WinWeelay.Core
{
    /// <summary>
    /// Entry in an Hdata structure.
    /// </summary>
    public class WeechatHdataEntry : WeechatRelayObject
    {
        private List<string> _pointers = new List<string>();
        private Dictionary<string, WeechatRelayObject> _data = new Dictionary<string, WeechatRelayObject>();

        /// <summary>
        /// Add a new pointer to the data entry.
        /// </summary>
        /// <param name="pointer">The pointer to add.</param>
        public void AddPointer(string pointer)
        {
            _pointers.Add(pointer);
        }

        /// <summary>
        /// Add a new object to the data entry.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        public void AddObject(string key, WeechatRelayObject value)
        {
            _data.Add(key, value);
        }

        /// <summary>
        /// Retrieves a relay object from the data entry.
        /// </summary>
        /// <param name="key">The key of the element to retrieve.</param>
        /// <returns>The value for the given key.</returns>
        public WeechatRelayObject this[string key] => _data[key];

        /// <summary>
        /// Retrieves the last pointer that was added.
        /// </summary>
        /// <returns>The last pointer that was added.</returns>
        public string GetPointer()
        {
            return _pointers.ElementAt(_pointers.Count - 1);
        }

        /// <summary>
        /// Retrieves the pointer at a given index.
        /// </summary>
        /// <param name="index">A given index.</param>
        /// <returns>the pointer at the given index.</returns>
        public string GetPointer(int index)
        {
            if (index >= _pointers.Count || index < 0)
                return null;

            return _pointers.ElementAt(index);
        }

        /// <summary>
        /// Check whether the data entry dictionary contains a given key.
        /// </summary>
        /// <param name="key">A given key.</param>
        /// <returns>Does the key exists in the dictionary?</returns>
        public bool DataContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        /// <summary>
        /// Get the hashtable of local variables if it is present.
        /// </summary>
        /// <returns>A hashtable of local variables.</returns>
        public WeechatHashtable GetLocalVariables()
        {
            WeechatHashtable localVars = new WeechatHashtable();
            if (DataContainsKey("local_variables"))
                localVars = (WeechatHashtable)this["local_variables"];

            return localVars;
        }

        /// <summary>
        /// Override for debug purposes.
        /// </summary>
        /// <param name="indent">A given intent.</param>
        /// <returns>All values in the Hdata entry.</returns>
        public string ToString(int indent)
        {
            string tmp = "";
            for (int i = 0; i < indent; i++)
                tmp += " ";

            string ret = $"{tmp}[HdataEntry]{Environment.NewLine}";
            ret += $"{tmp}  Pointers: {string.Join(", ", _pointers)}{Environment.NewLine}";

            foreach (KeyValuePair<string, WeechatRelayObject> pair in _data)
                ret += $"{tmp}  {pair.Key}={pair.Value}{Environment.NewLine}";

            return ret;
        }
    }
}
