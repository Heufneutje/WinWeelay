using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WinWeelay.Core
{
    /// <summary>
    /// Info list representation of a relay object.
    /// </summary>
    public class WeechatInfoList : WeechatRelayObject, IEnumerable
    {
        private List<Dictionary<string, WeechatRelayObject>> _items;

        /// <summary>
        /// Name of the info list.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The number of items in the info list.
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Create a new list of info objects.
        /// </summary>
        /// <param name="name">Name of the info list.</param>
        public WeechatInfoList(string name)
        {
            _items = new List<Dictionary<string, WeechatRelayObject>>();
            Name = name;
            Type = WeechatType.INL;
        }

        /// <summary>
        /// Add a new info object to the list.
        /// </summary>
        /// <param name="variables">Dictionary of info values.</param>
        public void AddItem(Dictionary<string, WeechatRelayObject> variables)
        {
            _items.Add(variables);
        }

        /// <summary>
        /// Returns a dictionary of info values at a given index.
        /// </summary>
        /// <param name="index">A given index.</param>
        /// <returns>A dictionary of info values</returns>
        public Dictionary<string, WeechatRelayObject> this[int index] => _items[index];

        /// <summary>
        /// Override for debug purposes.
        /// </summary>
        /// <returns>The key and value of the pair.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Name + $":{Environment.NewLine}");
            foreach (Dictionary<string, WeechatRelayObject> item in _items)
            {
                foreach (KeyValuePair<string, WeechatRelayObject> pair in item)
                    sb.Append($"  {pair.Key}->{pair.Value}, ");

                sb.Append($"{Environment.NewLine}{Environment.NewLine}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// IEnumerable implementation.
        /// </summary>
        /// <returns>Enumerator for looping.</returns>
        public IEnumerator GetEnumerator()
        {
            for (int index = 0; index < _items.Count; index++)
                yield return _items[index];
        }
    }
}
