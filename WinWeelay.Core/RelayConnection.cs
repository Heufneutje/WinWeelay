using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private IBufferView _bufferView;
        
        public RelayInputHandler InputHandler { get; private set; }
        public RelayOutputHandler OutputHandler { get; private set; }
        public ObservableCollection<RelayBuffer> Buffers { get; private set; }
        public RelayConfiguration Configuration { get; private set; }
        public RelayBuffer ActiveBuffer { get; set; }
        public bool IsConnected { get; private set; }

        public RelayConnection() { }

        public RelayConnection(IBufferView view, RelayConfiguration configuration)
        {
            Buffers = new ObservableCollection<RelayBuffer>();
            Configuration = configuration;

            _hostname = configuration.Hostname;
            _port = configuration.Port;
            _relayPassword = configuration.DecryptedRelayPassword;
            _bufferView = view;
        }

        public void Connect()
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(_hostname, _port);
            _networkStream = _tcpClient.GetStream();

            IsConnected = true;
            NotifyPropertyChanged(nameof(IsConnected));

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

            foreach (RelayBuffer buffer in Buffers)
                CloseBuffer(buffer);

            Buffers.Clear();
            NotifyBuffersChanged();

            ActiveBuffer = null;
            NotifyNicklistUpdated();

            IsConnected = false;
            NotifyPropertyChanged(nameof(IsConnected));
        }

        public void SortBuffers()
        {
            Buffers = new ObservableCollection<RelayBuffer>(Buffers.OrderBy(x => x.Number));
        }

        public void CloseBuffer(RelayBuffer buffer)
        {
            _bufferView.CloseBuffer(buffer);
        }

        public void NotifyBufferClosed(RelayBuffer buffer)
        {
            _bufferView.CloseBuffer(buffer);
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
