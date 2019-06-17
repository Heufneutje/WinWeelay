using System.Collections.Generic;
using System.Text;

namespace WinWeeRelay.Core
{
    public class WeechatHashtable : WinWeeRelayObject
    {
        private Dictionary<string, WinWeeRelayObject> _dict = new Dictionary<string, WinWeeRelayObject>();

        public void Add(WinWeeRelayObject key, WinWeeRelayObject value)
        {
            _dict.Add(key.ToString(), value);
        }

        public WinWeeRelayObject this[string key]
        {
            get
            {
                return _dict[key];
            }
        }

        public override string ToString()
        {
            StringBuilder map = new StringBuilder();
            foreach (KeyValuePair<string, WinWeeRelayObject> pair in _dict)
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
