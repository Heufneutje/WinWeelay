using WinWeelay.Configuration;

namespace WinWeelay.Core
{
    /// <summary>
    /// Factory to create an instance of a transport based on the chosen connection type.
    /// </summary>
    public static class RelayTransportFactory
    {
        /// <summary>
        /// Create an instance of a transport based on the chosen connection type.
        /// </summary>
        /// <param name="connectionType">The chosen connection type.</param>
        /// <returns>An instance of the transport<./returns>
        public static IRelayTransport GetTransport(RelayConnectionType connectionType)
        {
            switch (connectionType)
            {
                case RelayConnectionType.PlainText:
                    return new TcpRelayTransport();
                case RelayConnectionType.WeechatSsl:
                    return new SslRelayTransport();
                case RelayConnectionType.WebSocket:
                    return new WebSocketRelayTransport(false);
                case RelayConnectionType.WebSocketSsl:
                    return new WebSocketRelayTransport(true);
                default:
                    return null;
            }
        }
    }
}
