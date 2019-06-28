using System;

namespace WinWeelay.Core
{
    public class RelayBufferMessageEventArgs : EventArgs
    {
        public RelayBufferMessage Message { get; private set; }
        public bool AddToEnd { get; private set; }

        public RelayBufferMessageEventArgs(RelayBufferMessage message, bool addToEnd)
        {
            Message = message;
            AddToEnd = addToEnd;
        }
    }
}
