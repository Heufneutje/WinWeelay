using System;

namespace WinWeelay.Core
{
    public class RelayBufferMessageEventArgs : EventArgs
    {
        public RelayBufferMessage Message { get; private set; }
        public bool AddToEnd { get; private set; }
        public bool IsExpandedBacklog { get; private set; }

        public RelayBufferMessageEventArgs(RelayBufferMessage message, bool addToEnd, bool isExpandedBacklog)
        {
            Message = message;
            AddToEnd = addToEnd;
            IsExpandedBacklog = isExpandedBacklog;
        }
    }
}
