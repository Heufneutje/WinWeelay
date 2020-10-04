using System;

namespace WinWeelay.Core
{
    /// <summary>
    /// Arguments for a received buffer message.
    /// </summary>
    public class RelayBufferMessageEventArgs : EventArgs
    {
        /// <summary>
        /// The message that is received.
        /// </summary>
        public RelayBufferMessage Message { get; private set; }

        /// <summary>
        /// True if the message should be added to the bottom of the buffer, false if it should be added to the top.
        /// </summary>
        public bool AddToEnd { get; private set; }

        /// <summary>
        /// True if the messages are received after the "Load more messages" action.
        /// </summary>
        public bool IsExpandedBacklog { get; private set; }

        /// <summary>
        /// Create arguments for a received message.
        /// </summary>
        /// <param name="message">The message that is received.</param>
        /// <param name="addToEnd">True if the message should be added to the bottom of the buffer, false if it should be added to the top.</param>
        /// <param name="isExpandedBacklog">True if the messages are received after the "Load more messages" action.</param>
        public RelayBufferMessageEventArgs(RelayBufferMessage message, bool addToEnd, bool isExpandedBacklog)
        {
            Message = message;
            AddToEnd = addToEnd;
            IsExpandedBacklog = isExpandedBacklog;
        }
    }
}
