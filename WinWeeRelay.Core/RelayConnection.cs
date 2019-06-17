using System.Collections.Generic;
using System.Net.Sockets;

namespace WinWeeRelay.Core
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

        public RelayConnection() { }

        public RelayConnection(string hostname, int port, string relayPassword)
        {
            _tcpClient = new TcpClient();
            _hostname = hostname;
            _port = port;
            _relayPassword = relayPassword;
            Buffers = new List<RelayBuffer>();
        }

        public void Connect()
        {
            _tcpClient.Connect(_hostname, _port);
            _networkStream = _tcpClient.GetStream();

            InputHandler = new RelayInputHandler(this, _networkStream);
            OutputHandler = new RelayOutputHandler(_networkStream);

            OutputHandler.Init(_relayPassword);
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
    }
}
