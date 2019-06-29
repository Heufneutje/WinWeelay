using System;
using System.Collections.Generic;
using Ionic.Zlib;

namespace WinWeelay.Core
{
    public class RelayMessage
    {
        public List<WeechatRelayObject> RelayObjects { get; private set; }
        public bool Compressed { get; private set; }
        public int Length { get; private set; }
        public string ID { get; private set; }

        public RelayMessage(byte[] data)
        {
            RelayObjects = new List<WeechatRelayObject>();

            WeechatData wd = new WeechatData(data);

            Length = wd.GetUnsignedInt();

            int c = wd.GetByte();
            if (c == 0x00)
                Compressed = false;
            else if (c == 0x01)
            {
                Compressed = true;
                wd = new WeechatData(ZlibStream.UncompressBuffer(wd.GetByteArray()));
            }

            ID = wd.GetString();

            while (!wd.IsEmpty)
                RelayObjects.Add(wd.GetObject());
        }

        public override string ToString()
        {
            string msg = $"[WMessage.tostring]{Environment.NewLine}  Length: {Length}{Environment.NewLine}  Compressed: {Compressed}{Environment.NewLine}  ID: {ID}{Environment.NewLine}";
            foreach (WeechatRelayObject obj in RelayObjects)
                msg += $"{obj}{Environment.NewLine}";
            return msg;
        }
    }
}
