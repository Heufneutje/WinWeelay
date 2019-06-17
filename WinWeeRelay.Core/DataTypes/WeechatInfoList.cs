using System;
using System.Collections.Generic;
using System.Text;

namespace WinWeeRelay.Core
{
    public class WeechatInfoList : WinWeeRelayObject
    {
        public string Name { get; private set; }
        private List<Dictionary<string, WinWeeRelayObject>> _items = new List<Dictionary<string, WinWeeRelayObject>>();

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

        public void AddItem(Dictionary<string, WinWeeRelayObject> variables)
        {
            _items.Add(variables);
        }

        public Dictionary<string, WinWeeRelayObject> this[int index]
        {
            get
            {
                return _items[index];
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Name + $":{Environment.NewLine}");
            foreach (Dictionary<string, WinWeeRelayObject> item in _items)
            {
                foreach (KeyValuePair<string, WinWeeRelayObject> pair in item)
                    sb.Append($"  {pair.Key}->{pair.Value}, ");

                sb.Append($"{Environment.NewLine}{Environment.NewLine}");
            }
            return sb.ToString();
        }
    }
}
