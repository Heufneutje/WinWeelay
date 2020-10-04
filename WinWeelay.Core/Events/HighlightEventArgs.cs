using System;

namespace WinWeelay.Core
{
    /// <summary>
    /// Arguments to specify which message triggered a highlight.
    /// </summary>
    public class HighlightEventArgs : EventArgs
    {
        /// <summary>
        /// The message which triggered the highlight.
        /// </summary>
        public RelayBufferMessage Message { get; private set; }

        /// <summary>
        /// The buffer the highlight was triggered in.
        /// </summary>
        public RelayBuffer Buffer { get; private set; }

        /// <summary>
        /// Create arguments for a highlight.
        /// </summary>
        /// <param name="message">The message which triggered the highlight.</param>
        /// <param name="buffer">The buffer the highlight was triggered in.</param>
        public HighlightEventArgs(RelayBufferMessage message, RelayBuffer buffer)
        {
            Message = message;
            Buffer = buffer;
        }
    }
}
