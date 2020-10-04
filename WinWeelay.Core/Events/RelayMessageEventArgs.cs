using System;

namespace WinWeelay.Core
{
    /// <summary>
    /// Arguments for a received raw message.
    /// </summary>
    public class RelayMessageEventArgs : EventArgs
    {
        /// <summary>
        /// The raw message that is received.
        /// </summary>
        public RelayMessage RelayMessage { get; }

        /// <summary>
        /// Create arguments for a received raw message.
        /// </summary>
        /// <param name="relayMessage">The raw message that is received.</param>
        public RelayMessageEventArgs(RelayMessage relayMessage)
        {
            RelayMessage = relayMessage;
        }
    }
}
