using System;
using System.Collections.Generic;
using System.Linq;

namespace WinWeeRelay.Core
{
    public class WeechatHdataEntry : WinWeeRelayObject
    {
        private List<string> _pointers = new List<string>();
        private Dictionary<string, WinWeeRelayObject> _data = new Dictionary<string, WinWeeRelayObject>();

        public void AddPointer(string pointer)
        {
            _pointers.Add(pointer);
        }

        public void AddObject(string key, WinWeeRelayObject value)
        {
            _data.Add(key, value);
        }

        public string ToString(int indent)
        {
            string tmp = "";
            for (int i = 0; i < indent; i++)
                tmp += " ";

            string ret = $"{tmp}[HdataEntry]{Environment.NewLine}";
            ret += $"{tmp}  Pointers: {string.Join(", ", _pointers)}{Environment.NewLine}";

            foreach (KeyValuePair<string, WinWeeRelayObject> pair in _data)
                ret += $"{tmp}  {pair.Key}={pair.Value}{Environment.NewLine}";

            return ret;
        }

        public WinWeeRelayObject this[string key]
        {
            get
            {
                return _data[key];
            }
        }

        public string GetPointer()
        {
            return _pointers.ElementAt(_pointers.Count - 1);
        }

        public long GetPointerLong()
        {
            try
            {
                return Convert.ToInt64(GetPointer().Substring(2), 16);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public string GetPointer(int index)
        {
            return _pointers.ElementAt(index);
        }

        public long GetPointerLong(int index)
        {
            try
            {
                return Convert.ToInt64(GetPointer(index).Substring(2), 16);
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
