using System;

namespace WinWeelay.Core
{
    public class RelayMessageEventArgs : EventArgs
    {
        public RelayMessage RelayMessage { get; }

        public RelayMessageEventArgs(RelayMessage relayMessage)
        {
            RelayMessage = relayMessage;
        }
    }
}
