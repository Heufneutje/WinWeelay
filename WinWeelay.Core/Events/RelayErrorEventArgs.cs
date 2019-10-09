using System;

namespace WinWeelay.Core
{
    public class RelayErrorEventArgs : EventArgs
    {
        public Exception Error { get; }

        public RelayErrorEventArgs(Exception error)
        {
            Error = error;
        }
    }
}
