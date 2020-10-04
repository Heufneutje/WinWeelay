using System;

namespace WinWeelay.Core
{
    /// <summary>
    /// Arguments to specify an error.
    /// </summary>
    public class RelayErrorEventArgs : EventArgs
    {
        /// <summary>
        /// The error that has occurred.
        /// </summary>
        public Exception Error { get; }

        /// <summary>
        /// Create arguments to specify an error.
        /// </summary>
        /// <param name="error">The error that has occurred.</param>
        public RelayErrorEventArgs(Exception error)
        {
            Error = error;
        }
    }
}
