using System;

namespace WinWeelay.Core
{
    public class ConnectionLostEventArgs : EventArgs
    {
        public Exception Error { get; private set; }

        public ConnectionLostEventArgs(Exception error)
        {
            Error = error;
        }
    }
}
