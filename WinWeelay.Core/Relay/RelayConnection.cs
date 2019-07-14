using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;
using WinWeelay.Configuration;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    public class RelayConnection : NotifyPropertyChangedBase
    {
        private TcpClient _tcpClient;
        private Stream _networkStream;
        private IBufferView _bufferView;
        private Timer _pingTimer;
        
        public RelayInputHandler InputHandler { get; private set; }
        public RelayOutputHandler OutputHandler { get; private set; }
        public ObservableCollection<RelayBuffer> Buffers { get; private set; }
        public RelayConfiguration Configuration { get; set; }
        public RelayBuffer ActiveBuffer { get; set; }
        public OptionParser OptionParser { get; set; }
        public bool IsConnected { get; private set; }

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
            OptionParser = new OptionParser();
        }

        public RelayConnection(IBufferView view, RelayConfiguration configuration)
        {
            Buffers = new ObservableCollection<RelayBuffer>();
            Configuration = configuration;
            OptionParser = new OptionParser();

            _bufferView = view;
            _pingTimer = new Timer(30000) { AutoReset = true };
            _pingTimer.Elapsed += PingTimer_Elapsed;
        }

        public async Task<bool> Connect()
        {
            _tcpClient = new TcpClient();

            try
            {
                await _tcpClient.ConnectAsync(Configuration.Hostname, Configuration.Port);
                switch (Configuration.ConnectionType)
                {
                    case RelayConnectionType.PlainText:
                        _networkStream = _tcpClient.GetStream();
                        break;
                    case RelayConnectionType.WeechatSsl:
                        _networkStream = new SslStream(_tcpClient.GetStream());
                        SslStream sslStream = _networkStream as SslStream;

                        await Task.WhenAny(sslStream.AuthenticateAsClientAsync(Configuration.Hostname), Task.Delay(5000));
                        if (!sslStream.IsAuthenticated)
                            throw new IOException("SSL authentication timed out.");
                        break;
                }
            }
            catch (Exception ex)
            {
                _networkStream?.Dispose();
                _tcpClient?.Dispose();
                HandleException(ex);
                return false;
            }

            IsConnected = true;
            NotifyPropertyChanged(nameof(IsConnected));

            InputHandler = new RelayInputHandler(this, _networkStream);
            OutputHandler = new RelayOutputHandler(this, _networkStream);

            OutputHandler.BeginMessageBatch();
            OutputHandler.Init(Cipher.Decrypt(Configuration.RelayPassword), true);
            OutputHandler.RequestBufferList();
            OutputHandler.RequestHotlist();
            OutputHandler.Sync();
            OutputHandler.Info("version", MessageIds.CustomGetVersion);
            OutputHandler.RequestColorOptions();
            OutputHandler.EndMessageBatch();

            _pingTimer.Start();
            return true;
        }

        public void Disconnect(bool cleanDisconnect)
        {
            IsConnected = false;
            WeeChatVersion = null;
            NotifyPropertyChanged(nameof(Description));

            if (cleanDisconnect)
            {
                try
                {
                    OutputHandler.Quit();
                    _networkStream.Dispose();
                    _tcpClient.Dispose();
                }
                catch (IOException) { } // Ignore this since we want to disconnect anyway.
            }

            InputHandler.CancelInputWorker();

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

            if (ex is IOException || ex is SocketException)
                ConnectionLost?.Invoke(this, new ConnectionLostEventArgs(error));
            else
                throw ex;
        }

        public void OnHighlighted(RelayBufferMessage message, RelayBuffer buffer)
        {
            Highlighted?.Invoke(this, new HighlightEventArgs(message, buffer));
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
