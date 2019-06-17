using System.Collections.Generic;
using System.Text;

namespace WinWeelay.Core
{
    public class WeechatHashtable : WeechatRelayObject
    {
        private Dictionary<string, WeechatRelayObject> _dict = new Dictionary<string, WeechatRelayObject>();

        public void Add(WeechatRelayObject key, WeechatRelayObject value)
        {
            _dict.Add(key.ToString(), value);
        }

        public WeechatRelayObject this[string key]
        {
            get
            {
                return _dict[key];
            }
        }

        public override string ToString()
        {
            StringBuilder map = new StringBuilder();
            foreach (KeyValuePair<string, WeechatRelayObject> pair in _dict)
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
