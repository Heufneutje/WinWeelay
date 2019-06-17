using System;
using System.Collections.Generic;

namespace WinWeeRelay.Core
{
    public class RelayMessage
    {
        public List<WinWeeRelayObject> RelayObjects { get; private set; }
        public bool Compressed { get; private set; }
        public int Length { get; private set; }
        public string ID { get; private set; }

        public RelayMessage(byte[] data)
        {
            RelayObjects = new List<WinWeeRelayObject>();

            WeechatData wd = new WeechatData(data);

            Length = wd.GetUnsignedInt();

            int c = wd.GetByte();
            if (c == 0x00)
                Compressed = false;

            ID = wd.GetString();

            while (!wd.IsEmpty)
                RelayObjects.Add(wd.GetObject());
        }

        public override string ToString()
        {
            string msg = $"[WMessage.tostring]{Environment.NewLine}  Length: {Length}{Environment.NewLine}  Compressed: {Compressed}{Environment.NewLine}  ID: {ID}{Environment.NewLine}";
            foreach (WinWeeRelayObject obj in RelayObjects)
                msg += $"{obj}{Environment.NewLine}";
            return msg;
        }
    }
}
