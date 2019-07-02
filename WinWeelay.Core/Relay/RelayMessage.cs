using System;
using System.Collections.Generic;
using Ionic.Zlib;

namespace WinWeelay.Core
{
    /// <summary>
    /// A message parsed from relay input data.
    /// </summary>
    public class RelayMessage
    {
        /// <summary>
        /// A collection of relay objects contained in the message data.
        /// </summary>
        public List<WeechatRelayObject> RelayObjects { get; private set; }

        /// <summary>
        /// True if the message is zlib compressed, false if the message is not compressed.
        /// </summary>
        public bool Compressed { get; private set; }

        /// <summary>
        /// The number of bytes in the data array.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// ID of the relay message.
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Initialize a new relay message.
        /// </summary>
        /// <param name="data">Data array received from the relay input stream.</param>
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

        /// <summary>
        /// Override for debug purposes.
        /// </summary>
        /// <returns>A string representation of the entire relay message.</returns>
        public override string ToString()
        {
            string msg = $"[WMessage.tostring]{Environment.NewLine}  Length: {Length}{Environment.NewLine}  Compressed: {Compressed}{Environment.NewLine}  ID: {ID}{Environment.NewLine}";
            foreach (WeechatRelayObject obj in RelayObjects)
                msg += $"{obj}{Environment.NewLine}";
            return msg;
        }
    }
}
