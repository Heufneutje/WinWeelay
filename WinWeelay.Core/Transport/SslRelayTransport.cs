using System.IO;
using System.Net.Security;
using System.Threading.Tasks;

namespace WinWeelay.Core
{
    /// <summary>
    /// Secure TCP connection with SSL.
    /// </summary>
    public class SslRelayTransport : TcpRelayTransport
    {
        /// <summary>
        /// Initialize the network stream and vrify the SSL connection.
        /// </summary>
        /// <returns>Async task.</returns>
        protected override async Task InitializeStream()
        {
            _networkStream = new SslStream(_tcpClient.GetStream());
            SslStream sslStream = _networkStream as SslStream;

            await Task.WhenAny(sslStream.AuthenticateAsClientAsync(_configuration.Hostname), Task.Delay(5000));
            if (!sslStream.IsAuthenticated)
                throw new IOException("SSL authentication timed out.");
        }
    }
}
