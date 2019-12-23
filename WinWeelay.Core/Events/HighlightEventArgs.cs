using System;

namespace WinWeelay.Core
{
    public class HighlightEventArgs : EventArgs
    {
        public RelayBufferMessage Message { get; private set; }
        public RelayBuffer Buffer { get; private set; }

        public HighlightEventArgs(RelayBufferMessage message, RelayBuffer buffer)
        {
            Message = message;
            Buffer = buffer;
        }
    }
}
