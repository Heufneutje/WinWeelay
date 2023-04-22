using System.Collections;
using System.Collections.Generic;

namespace WinWeelay.Core
{
    /// <summary>
    /// Info list representation of a relay object.
    /// </summary>
    public class WeechatInfoList : WeechatRelayObject, IEnumerable
    {
        private readonly List<Dictionary<string, WeechatRelayObject>> _items;

        /// <summary>
        /// Name of the info list.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Create a new list of info objects.
        /// </summary>
        /// <param name="name">Name of the info list.</param>
        public WeechatInfoList(string name)
        {
            _items = new List<Dictionary<string, WeechatRelayObject>>();
            Name = name;
            Type = WeechatType.Infolist;
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
