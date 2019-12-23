using System;
using System.Collections.Generic;

namespace WinWeelay.Core
{
    public class RelayBufferMessageBatchEventsArgs : EventArgs
    {
        public IEnumerable<RelayBufferMessage> Messages { get; private set; }
        public bool AddToEnd { get; private set; }
        public bool IsExpandedBacklog { get; private set; }

        public RelayBufferMessageBatchEventsArgs(IEnumerable<RelayBufferMessage> messages, bool addToEnd, bool isExpandedBacklog)
        {
            Messages = messages;
            AddToEnd = addToEnd;
            IsExpandedBacklog = isExpandedBacklog;
        }
    }
}
