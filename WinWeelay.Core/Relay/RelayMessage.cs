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
        /// Header containing ID and length.
        /// </summary>
        public RelayMessageHeader Header { get; private set; }

        /// <summary>
        /// Initialize a new relay message.
        /// </summary>
        /// <param name="data">Data array received from the relay input stream.</param>
        public RelayMessage(byte[] data)
        {
            RelayDataParser parser = new RelayDataParser(data);
            Header = parser.GetHeader();
            RelayObjects = parser.GetObjects();
        }
    }
}
