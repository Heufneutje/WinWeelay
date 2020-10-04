using System;
using System.Collections.Generic;

namespace WinWeelay.Core
{
    /// <summary>
    /// Arguments for a received number of buffer messages.
    /// </summary>
    public class RelayBufferMessageBatchEventsArgs : EventArgs
    {
        /// <summary>
        /// The messages that are received.
        /// </summary>
        public IEnumerable<RelayBufferMessage> Messages { get; private set; }

        /// <summary>
        /// True if the messages should be added to the bottom of the buffer, false if they should be added to the top.
        /// </summary>
        public bool AddToEnd { get; private set; }

        /// <summary>
        /// True if the messages are received after the "Load more messages" action.
        /// </summary>
        public bool IsExpandedBacklog { get; private set; }

        /// <summary>
        /// Arguments for a received number of messages.
        /// </summary>
        /// <param name="messages">The messages that are received.</param>
        /// <param name="addToEnd">True if the messages should be added to the bottom of the buffer, false if they should be added to the top.</param>
        /// <param name="isExpandedBacklog">True if the messages are received after the "Load more messages" action.</param>
        public RelayBufferMessageBatchEventsArgs(IEnumerable<RelayBufferMessage> messages, bool addToEnd, bool isExpandedBacklog)
        {
            Messages = messages;
            AddToEnd = addToEnd;
            IsExpandedBacklog = isExpandedBacklog;
        }
    }
}
