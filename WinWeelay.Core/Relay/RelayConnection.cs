using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using WinWeelay.Configuration;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    /// <summary>
    /// Connection to the WeeChat host.
    /// </summary>
    public class RelayConnection : NotifyPropertyChangedBase
    {
        private IRelayTransport _transport;
        private IBufferWindow _bufferView;
        private Timer _pingTimer;
        private HashFactory _hashFactory;

        /// <summary>
        /// Whether the buffer list is beimg updated and visual updates should be prevented.
        /// </summary>
        public bool IsRefreshingBuffers { get; internal set; }
        
        /// <summary>
        /// The parser for incoming messages on this connection.
        /// </summary>
        public RelayInputHandler InputHandler { get; private set; }

        /// <summary>
        /// The handler for outgoing messages from this connection.
        /// </summary>
        public RelayOutputHandler OutputHandler { get; private set; }

        /// <summary>
        /// The buffers that have been loaded.
        /// </summary>
        public ObservableCollection<RelayBuffer> Buffers { get; private set; }

        /// <summary>
        /// The main configuration loaded from the config file.
        /// </summary>
        public RelayConfiguration Configuration { get; set; }

        /// <summary>
        /// The buffer that is currently active in the UI.
        /// </summary>
        public RelayBuffer ActiveBuffer => Buffers.FirstOrDefault(x => x.IsActiveBuffer);

        /// <summary>
        /// Parser to handle WeeChat options.
        /// </summary>
        public OptionParser OptionParser { get; set; }

        /// <summary>
        /// Whether the connection is currently active.
        /// </summary>
        public bool IsConnected => _transport != null && _transport.IsConnected;

        /// <summary>
        /// Whether an init command has been sent successfully.
        /// </summary>
        public bool IsLoggedIn { get; internal set; }

        /// <summary>
        /// Handler for IRC server capabilities.
        /// </summary>
        public IrcServerRegistry IrcServerRegistry { get; set; }

        private string _weeChatVersion;

        /// <summary>
        /// The version of WeeChat running on the host.
        /// </summary>
        public string WeeChatVersion
        {
            get => _weeChatVersion;
            set
            {
                _weeChatVersion = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }

        /// <summary>
        /// The title of the main window.
        /// </summary>
        public string Description
        {
            get
            {
                if (!IsConnected || string.IsNullOrEmpty(WeeChatVersion))
                    return "WinWeelay";

                return $"WinWeelay - WeeChat {WeeChatVersion}";
            }
        }

        /// <summary>
        /// The buffers which have no parent (core, plugins and servers).
        /// </summary>
        public ObservableCollection<RelayBuffer> RootBuffers => new ObservableCollection<RelayBuffer>(Buffers.Where(x => x.Parent == null));

        /// <summary>
        /// Event fired when the connection is terminated.
        /// </summary>
        public event ConnectionLostHandler ConnectionLost;

        /// <summary>
        /// Event fired when a message triggers a highlight.
        /// </summary>
        public event HighlightHandler Highlighted;

        /// <summary>
        /// Event fired when a retrieved options list has been parsed.
        /// </summary>
        public event EventHandler OptionsParsed;

        /// <summary>
        /// Constructor for designer.
        /// </summary>
        public RelayConnection()
        {
            Buffers = new ObservableCollection<RelayBuffer>();
            OptionParser = new OptionParser(Configuration);
            IrcServerRegistry = new IrcServerRegistry();
        }

        /// <summary>
        /// Create a new connection.
        /// </summary>
        /// <param name="view">Control that displays the buffer list (list or tree).</param>
        /// <param name="configuration">The main configuration loaded from the config file.</param>
        public RelayConnection(IBufferWindow view, RelayConfiguration configuration)
        {
            Buffers = new ObservableCollection<RelayBuffer>();
            Configuration = configuration;
            OptionParser = new OptionParser(Configuration);
            IrcServerRegistry = new IrcServerRegistry();

            _hashFactory = new HashFactory();
            _bufferView = view;
            _pingTimer = new Timer(30000) { AutoReset = true };
            _pingTimer.Elapsed += PingTimer_Elapsed;
        }

        /// <summary>
        /// Connect to the WeeChat host.
        /// </summary>
        /// <returns>Async result.</returns>
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

            if (Configuration.HandshakeType == HandshakeType.Legacy)
                Authenticate(null);
            else
                OutputHandler.Handshake(_hashFactory.GetSupportedAlgorithms());
            
            return true;
        }

        /// <summary>
        /// Disconnect from the WeeChat host.
        /// </summary>
        /// <param name="cleanDisconnect">Whether a quit command should be sent.</param>
        public void Disconnect(bool cleanDisconnect)
        {
            WeeChatVersion = null;
            IsLoggedIn = false;
            NotifyPropertyChanged(nameof(Description));

            if (cleanDisconnect)
            {
                try
                {
                    if (IsLoggedIn)
                        OutputHandler.Quit();
                    IrcServerRegistry.Clear();
                    _transport.Disconnect();
                }
                catch (IOException) { } // Ignore this since we want to disconnect anyway.
            }

            _pingTimer.Stop();
            NotifyPropertyChanged(nameof(IsConnected));
        }

        /// <summary>
        /// Authenticate with the relay.
        /// </summary>
        /// <param name="handshakeResult">The received parameters from the handshake command.</param>
        public void Authenticate(WeechatHashtable handshakeResult)
        {
            try
            {
                string commandParam;
                if (handshakeResult == null)
                    commandParam = _hashFactory.GetLegacyInitCommandParameter(Configuration.RelayPassword);
                else
                    commandParam = _hashFactory.GetInitCommandParameter(Configuration.RelayPassword, handshakeResult);

                OutputHandler.Init(commandParam);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return;
            }

            RunPostInitCommands();
        }

        private void RunPostInitCommands()
        {
            OutputHandler.BeginMessageBatch();
            OutputHandler.RequestBufferList();
            OutputHandler.RequestHotlist();
            OutputHandler.Sync();
            OutputHandler.Info("version", MessageIds.CustomGetVersion);

            if (!OptionParser.HasOptionCache)
                OutputHandler.RequestColorOptions();

            OutputHandler.EndMessageBatch();

            _pingTimer.Start();
        }

        /// <summary>
        /// Sort the buffer list based on buffer numbers.
        /// </summary>
        public void SortBuffers()
        {
            Buffers = new ObservableCollection<RelayBuffer>(Buffers.OrderBy(x => x.Number));
            foreach (RelayBuffer buffer in RootBuffers)
                buffer.SortChildren();
        }

        /// <summary>
        /// Close a given buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public void CloseBuffer(RelayBuffer buffer)
        {
            _bufferView.CloseBuffer(buffer);
        }

        /// <summary>
        /// Retrieve the buffer that is the core WeeChat buffer.
        /// </summary>
        /// <returns>The core buffer.</returns>
        public RelayBuffer GetCoreBuffer()
        {
            return Buffers.FirstOrDefault(x => x.BufferType == null && x.PluginType == "core");
        }

        /// <summary>
        /// Update the UI when the buffer list changes.
        /// </summary>
        public void NotifyBuffersChanged()
        {
            IsRefreshingBuffers = true;
            NotifyPropertyChanged(nameof(Buffers));
            NotifyPropertyChanged(nameof(RootBuffers));
            IsRefreshingBuffers = false;
        }

        /// <summary>
        /// Update the UI when the nickname list for the active buffer changes.
        /// </summary>
        public void NotifyNicklistUpdated()
        {
            NotifyPropertyChanged(nameof(ActiveBuffer));
        }

        /// <summary>
        /// Notify the UI that the connection has been lost.
        /// </summary>
        /// <param name="ex">The reason that the connected has been lost.</param>
        public void HandleException(Exception ex)
        {
            Exception error = ex;
            while (error.InnerException != null)
                error = ex.InnerException;

            ConnectionLost?.Invoke(this, new ConnectionLostEventArgs(error));
        }

        /// <summary>
        /// Fire an event when a message triggers a highlight.
        /// </summary>
        /// <param name="message">The message which triggered the highlight.</param>
        /// <param name="buffer">The buffer the highlight was triggered in.</param>
        public void OnHighlighted(RelayBufferMessage message, RelayBuffer buffer)
        {
            Highlighted?.Invoke(this, new HighlightEventArgs(message, buffer));
        }

        /// <summary>
        /// Fire an event when parsing the retrieved options is finished.
        /// </summary>
        public void OnOptionsParsed()
        {
            OptionsParsed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Notify the UI that the connection has been lost.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="args">Arguments which contain the error.</param>
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
