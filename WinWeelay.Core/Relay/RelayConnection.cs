using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;
using WinWeelay.Configuration;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    public class RelayConnection : NotifyPropertyChangedBase
    {
        private IRelayTransport _transport;
        private IBufferWindow _bufferView;
        private Timer _pingTimer;
        
        public RelayInputHandler InputHandler { get; private set; }
        public RelayOutputHandler OutputHandler { get; private set; }
        public ObservableCollection<RelayBuffer> Buffers { get; private set; }
        public RelayConfiguration Configuration { get; set; }
        public RelayBuffer ActiveBuffer { get; set; }
        public OptionParser OptionParser { get; set; }
        public bool IsConnected => _transport != null && _transport.IsConnected;

        private string _weeChatVersion;
        public string WeeChatVersion
        {
            get
            {
                return _weeChatVersion;
            }
            set
            {
                _weeChatVersion = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }

        public string Description
        {
            get
            {
                if (!IsConnected || string.IsNullOrEmpty(WeeChatVersion))
                    return "WinWeelay";

                return $"WinWeelay - WeeChat {WeeChatVersion}";
            }
        }

        public ObservableCollection<RelayBuffer> RootBuffers => new ObservableCollection<RelayBuffer>(Buffers.Where(x => x.Parent == null));

        public event ConnectionLostHandler ConnectionLost;
        public event HighlightHandler Highlighted;

        public RelayConnection()
        {
            Buffers = new ObservableCollection<RelayBuffer>();
            OptionParser = new OptionParser(Configuration);
        }

        public RelayConnection(IBufferWindow view, RelayConfiguration configuration)
        {
            Buffers = new ObservableCollection<RelayBuffer>();
            Configuration = configuration;
            OptionParser = new OptionParser(Configuration);

            _bufferView = view;
            _pingTimer = new Timer(30000) { AutoReset = true };
            _pingTimer.Elapsed += PingTimer_Elapsed;
        }

        public async Task<bool> Connect()
        {
            _transport = RelayTransportFactory.GetTransport(Configuration.ConnectionType);
            _transport.ErrorReceived += Transport_ErrorReceived;

            await _transport.Connect(Configuration);
            if (!_transport.IsConnected)
                return false;

            NotifyPropertyChanged(nameof(IsConnected));

            InputHandler = new RelayInputHandler(this, _transport);
            OutputHandler = new RelayOutputHandler(this, _transport);

            OutputHandler.BeginMessageBatch();
            OutputHandler.Init(Cipher.Decrypt(Configuration.RelayPassword), true);
            OutputHandler.RequestBufferList();
            OutputHandler.RequestHotlist();
            OutputHandler.Sync();
            OutputHandler.Info("version", MessageIds.CustomGetVersion);

            if (!OptionParser.HasOptionCache)
                OutputHandler.RequestColorOptions();
            
            OutputHandler.EndMessageBatch();

            _pingTimer.Start();
            return true;
        }

        public void Disconnect(bool cleanDisconnect)
        {
            WeeChatVersion = null;
            NotifyPropertyChanged(nameof(Description));

            if (cleanDisconnect)
            {
                try
                {
                    OutputHandler.Quit();
                    _transport.Disconnect();
                }
                catch (IOException) { } // Ignore this since we want to disconnect anyway.
            }

            foreach (RelayBuffer buffer in Buffers)
                CloseBuffer(buffer);

            Buffers.Clear();
            NotifyBuffersChanged();

            ActiveBuffer = null;
            NotifyNicklistUpdated();

            _pingTimer.Stop();
            NotifyPropertyChanged(nameof(IsConnected));
        }

        public void SortBuffers()
        {
            Buffers = new ObservableCollection<RelayBuffer>(Buffers.OrderBy(x => x.Number));
            foreach (RelayBuffer buffer in RootBuffers)
                buffer.SortChildren();
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
            NotifyPropertyChanged(nameof(RootBuffers));
        }

        public void NotifyNicklistUpdated()
        {
            NotifyPropertyChanged(nameof(ActiveBuffer));
        }

        public void HandleException(Exception ex)
        {
            Exception error = ex;
            while (error.InnerException != null)
                error = ex.InnerException;

            ConnectionLost?.Invoke(this, new ConnectionLostEventArgs(error));
        }

        public void OnHighlighted(RelayBufferMessage message, RelayBuffer buffer)
        {
            Highlighted?.Invoke(this, new HighlightEventArgs(message, buffer));
        }

        private void Transport_ErrorReceived(object sender, RelayErrorEventArgs args)
        {
            HandleException(args.Error);
        }

        private void PingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsConnected)
                _pingTimer.Stop();
            else
                OutputHandler.Ping();
        }
    }
}
