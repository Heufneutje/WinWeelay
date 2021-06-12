namespace WinWeelay.Configuration
{
    /// <summary>
    /// Types of handshake for authenticating with the relay.
    /// </summary>
    public enum HandshakeType
    {
        /// <summary>
        /// Send the relay password in plain text (required for compatibility with WeeChat &lt; 2.9.
        /// </summary>
        Legacy,

        /// <summary>
        /// Create and send a password hash if supported by the relay (requires WeeChat &gt;= 2.9).
        /// </summary>
        Modern
    }
}
