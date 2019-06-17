using System.Collections.Generic;
using System.Net.Sockets;
using WinWeelay.Configuration;

namespace WinWeelay.Core
{
    public class RelayConnection : NotifyPropertyChangedBase
    {
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private string _hostname;
        private int _port;
        private string _relayPassword;
        
        public RelayInputHandler InputHandler { get; private set; }
        public RelayOutputHandler OutputHandler { get; private set; }
        public List<RelayBuffer> Buffers { get; private set; }
        public RelayConfiguration Configuration { get; private set; }
        public RelayBuffer ActiveBuffer { get; set; }

        public RelayConnection() { }

        public RelayConnection(RelayConfiguration configuration)
        {
            _tcpClient = new TcpClient();
            Buffers = new List<RelayBuffer>();
            Configuration = configuration;

            _hostname = configuration.Hostname;
            _port = configuration.Port;
            _relayPassword = configuration.DecryptedRelayPassword;
        }

        public void Connect()
        {
            _tcpClient.Connect(_hostname, _port);
            _networkStream = _tcpClient.GetStream();

            InputHandler = new RelayInputHandler(this, _networkStream);
            OutputHandler = new RelayOutputHandler(_networkStream);

            OutputHandler.Init(_relayPassword);
            OutputHandler.RequestBufferList();
            OutputHandler.Sync();
        }

        public void Disconnect()
        {
            OutputHandler.Quit();
            _tcpClient.Close();
        }

        public void NotifyBuffersChanged()
        {
            NotifyPropertyChanged(nameof(Buffers));
        }

        public void NotifyNicklistUpdated()
        {
            NotifyPropertyChanged(nameof(ActiveBuffer));
        }
    }
}
