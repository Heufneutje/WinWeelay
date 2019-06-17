using System;
using System.Collections.Generic;

namespace WinWeelay.Core
{
    public class WeechatHdata : WinWeelayObject
    {
        private List<WeechatHdataEntry> _items;

        public string[] KeyList { get; set; }
        public string[] PathList { get; set; }
        public WeechatType[] TypeList { get; set; }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public WeechatHdata()
        {
            _items = new List<WeechatHdataEntry>();
        }

        public void AddItem(WeechatHdataEntry hde)
        {
            _items.Add(hde);
        }

        public void SetKeys(string[] keys)
        {
            KeyList = new string[keys.Length];
            TypeList = new WeechatType[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                string[] kt = keys[i].Split(new char[] { ':' });
                KeyList[i] = kt[0];
                TypeList[i] = (WeechatType)Enum.Parse(typeof(WeechatType), kt[1].ToUpper());
            }
        }

        public WeechatHdataEntry this[int index]
        {
            get
            {
                return _items[index];
            }
        }

        public override string ToString()
        {
            string s = $"[WHdata]{Environment.NewLine}  path=";
            if (PathList == null)
                s += "null";
            else
                foreach (string p in PathList)
                    s += p + "/";

            s += Environment.NewLine;
            foreach (WeechatHdataEntry hde in _items)
                s += hde.ToString(2) + Environment.NewLine;
            return s;
        }
    }
}
