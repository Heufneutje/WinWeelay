using System;
using System.Collections.Generic;
using System.Text;

namespace WinWeelay.Core
{
    public class WeechatInfoList : WinWeelayObject
    {
        public string Name { get; private set; }
        private List<Dictionary<string, WinWeelayObject>> _items = new List<Dictionary<string, WinWeelayObject>>();

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

        public void AddItem(Dictionary<string, WinWeelayObject> variables)
        {
            _items.Add(variables);
        }

        public Dictionary<string, WinWeelayObject> this[int index]
        {
            get
            {
                return _items[index];
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Name + $":{Environment.NewLine}");
            foreach (Dictionary<string, WinWeelayObject> item in _items)
            {
                foreach (KeyValuePair<string, WinWeelayObject> pair in item)
                    sb.Append($"  {pair.Key}->{pair.Value}, ");

                sb.Append($"{Environment.NewLine}{Environment.NewLine}");
            }
            return sb.ToString();
        }
    }
}
