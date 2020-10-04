using System;

namespace WinWeelay.Core
{
    /// <summary>
    /// Arguments to specify the reason the connection was lost.
    /// </summary>
    public class ConnectionLostEventArgs : EventArgs
    {
        /// <summary>
        /// The reason the connection was lost.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Create arguments for the reason the connection was lost.
        /// </summary>
        /// <param name="error">The reason the connection was lost.</param>
        public ConnectionLostEventArgs(Exception error)
        {
            Error = error;
        }
    }
}
