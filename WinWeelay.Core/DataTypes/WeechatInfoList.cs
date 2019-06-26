using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WinWeelay.Core
{
    public class WeechatInfoList : WeechatRelayObject, IEnumerable
    {
        public string Name { get; private set; }
        private List<Dictionary<string, WeechatRelayObject>> _items = new List<Dictionary<string, WeechatRelayObject>>();

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public WeechatInfoList(string name)
        {
            Name = name;
            Type = WeechatType.INL;
        }

        public void AddItem(Dictionary<string, WeechatRelayObject> variables)
        {
            _items.Add(variables);
        }

        public Dictionary<string, WeechatRelayObject> this[int index]
        {
            get
            {
                return _items[index];
            }
        }

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

        public IEnumerator GetEnumerator()
        {
            for (int index = 0; index < _items.Count; index++)
                yield return _items[index];
        }
    }
}
