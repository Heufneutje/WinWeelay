using System;

namespace WinWeeRelay.Core
{
    public class WeechatInfo : WinWeeRelayObject
    {
        public string Value { get; private set; }
        public string Name { get; private set; }

        public WeechatInfo(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"[WInfo]:{Environment.NewLine}  " + Name + " -> " + Value;
        }
    }
}
