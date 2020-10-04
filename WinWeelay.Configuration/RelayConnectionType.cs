namespace WinWeelay.Configuration
{
    /// <summary>
    /// The type of relay connection.
    /// </summary>
    public enum RelayConnectionType
    {
        /// <summary>
        /// Plain TCP connection without SSL.
        /// </summary>
        PlainText = 0,

        /// <summary>
        /// Secure TCP connection with SSL.
        /// </summary>
        WeechatSsl = 1,

        /// <summary>
        /// Plain text WebSocket connection.
        /// </summary>
        WebSocket = 2,

        /// <summary>
        /// Secure TCP WebSocket with SSL.
        /// </summary>
        WebSocketSsl = 3
    }
}
