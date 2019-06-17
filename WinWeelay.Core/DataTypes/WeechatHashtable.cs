using System.Collections.Generic;
using System.Text;

namespace WinWeelay.Core
{
    public class WeechatHashtable : WinWeelayObject
    {
        private Dictionary<string, WinWeelayObject> _dict = new Dictionary<string, WinWeelayObject>();

        public void Add(WinWeelayObject key, WinWeelayObject value)
        {
            _dict.Add(key.ToString(), value);
        }

        public WinWeelayObject this[string key]
        {
            get
            {
                return _dict[key];
            }
        }

        public override string ToString()
        {
            StringBuilder map = new StringBuilder();
            foreach (KeyValuePair<string, WinWeelayObject> pair in _dict)
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
