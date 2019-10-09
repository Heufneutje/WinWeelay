using System.IO;
using System.Net.Security;
using System.Threading.Tasks;

namespace WinWeelay.Core
{
    public class SslRelayTransport : TcpRelayTransport
    {
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
