using WinWeelay.Configuration;

namespace WinWeelay.Core
{
    public static class RelayTransportFactory
    {
        public static IRelayTransport GetTransport(RelayConnectionType connectionType)
        {
            switch (connectionType)
            {
                case RelayConnectionType.PlainText:
                    return new TcpRelayTransport();
                case RelayConnectionType.WeechatSsl:
                    return new SslRelayTransport();
                default:
                    return null;
            }
        }
    }
}
