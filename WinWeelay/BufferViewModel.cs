using System;
using System.Timers;
using WinWeelay.Configuration;
using WinWeelay.Core;
using WinWeelay.Utils;

namespace WinWeelay
{
    public class BufferViewModel : NotifyPropertyChangedBase
    {
        private MainWindow _mainWindow;
        private RelayConfiguration _relayConfiguration;
        private Timer _retryTimer;
        private bool _isRetryingConnection;

        public RelayConnection Connection { get; private set; }
        public string ConnectionStatus { get; set; }

        public DelegateCommand ConnectCommand { get; private set; }
        public DelegateCommand DisconnectCommand { get; private set; }
        public DelegateCommand HideBufferCommand { get; private set; }
        public DelegateCommand CloseBufferCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand SettingsCommand { get; private set; }
        public DelegateCommand StopConnectingCommand { get; private set; }

        public BufferViewModel() { }

        public BufferViewModel(MainWindow window)
        {
            _mainWindow = window;
            SetStatusText("Disconnected.");

            _retryTimer = new Timer(10000);
            _retryTimer.Elapsed += RetryTimer_Elapsed;

            _relayConfiguration = ConfigurationHelper.LoadConfiguration();
            Connection = new RelayConnection(window, _relayConfiguration);

            ConnectCommand = new DelegateCommand(Connect, CanConnect);
            DisconnectCommand = new DelegateCommand(Disconnect, CanDisconnect);
            HideBufferCommand = new DelegateCommand(HideBuffer, CanCloseHide);
            CloseBufferCommand = new DelegateCommand(CloseBuffer, CanCloseHide);
            ExitCommand = new DelegateCommand(Exit);
            SettingsCommand = new DelegateCommand(ShowSettingsForm);
            StopConnectingCommand = new DelegateCommand(StopConnecting, CanStopConnecting);

            Connection.ConnectionLost += Connection_ConnectionLost;

            if (_relayConfiguration.AutoConnect)
                Connect(null);
        }

        private void Connection_ConnectionLost(object sender, ConnectionLostEventArgs args)
        {
            _mainWindow.Dispatcher.Invoke(() => 
            {
                bool wasConnected = Connection.IsConnected || _isRetryingConnection;

                if (wasConnected)
                    SetStatusText($"Connection lost, reason: {args.Error.Message}{(args.Error.Message.EndsWith(".") ? "" : ".")} Retrying in 10 seconds...");
                else
                    SetStatusText($"Connection failed, reason: {args.Error.Message}{(args.Error.Message.EndsWith(".") ? "" : ".")}");

                if (CanDisconnect(null))
                    Connection.Disconnect(false);
                UpdateConnectionCommands();

                if (wasConnected)
                {
                    _isRetryingConnection = true;
                    _retryTimer.Start();
                }
            });
        }

        private void RetryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _mainWindow.Dispatcher.Invoke(() =>
            {
                _retryTimer.Stop();
                Connect(null);
            });
        }

        private void ShowSettingsForm(object parameter)
        {
            RelayConfiguration config = new RelayConfiguration();
            _relayConfiguration.CopyPropertiesTo(config);

            SettingsWindow settingsWindow = new SettingsWindow(config) { Owner = _mainWindow };
            if (settingsWindow.ShowDialog() == true)
            {
                _relayConfiguration = settingsWindow.Configuration;
                Connection.Configuration = _relayConfiguration;

                if (config.HasPropertyChanged(nameof(config.FontFamily)) || config.HasPropertyChanged(nameof(config.FontSize)))
                    _mainWindow.UpdateFont();
            }
        }

        private bool CanConnect(object parameter)
        {
            return !Connection.IsConnected;
        }

        private async void Connect(object parameter)
        {
            if (string.IsNullOrEmpty(_relayConfiguration.Hostname) || _relayConfiguration.Port == 0)
            {
                SetStatusText($"Connection failed, reason: configuration is invalid.");
                return;
            }

            UpdateConnectionCommands();
            SetStatusText($"Attempting to connect to {_relayConfiguration.ConnectionAddress}...");

            if (await Connection.Connect())
            {
                _isRetryingConnection = false;
                UpdateConnectionCommands();
                SetStatusText($"Connected to {_relayConfiguration.ConnectionAddress}{(_relayConfiguration.ConnectionType == RelayConnectionType.WeechatSsl ? " (secure connection)" : "")}.");
            }
            else if (_isRetryingConnection)
                _retryTimer.Start();
        }

        private bool CanDisconnect(object parameter)
        {
            return Connection.IsConnected;
        }

        private void Disconnect(object parameter)
        {
            if (CanDisconnect(parameter))
                Connection.Disconnect(true);
            UpdateConnectionCommands();
            SetStatusText($"Disconnected.");
        }

        private bool CanStopConnecting(object parameter)
        {
            return _isRetryingConnection;
        }

        private void StopConnecting(object parameter)
        {
            _isRetryingConnection = false;
            _retryTimer.Stop();
            UpdateConnectionCommands();
            SetStatusText("Disconnected.");
        }

        private bool CanCloseHide(object parameter)
        {
            return Connection.ActiveBuffer != null;
        }

        private void CloseBuffer(object parameter)
        {
            Connection.OutputHandler.Input(Connection.ActiveBuffer, @"/close");
        }

        private void HideBuffer(object parameter)
        {
            Connection.CloseBuffer(Connection.ActiveBuffer);
        }

        private void Exit(object parameter)
        {
            _mainWindow.Close();
        }

        private void UpdateConnectionCommands()
        {
            ConnectCommand.OnCanExecuteChanged();
            DisconnectCommand.OnCanExecuteChanged();
            StopConnectingCommand.OnCanExecuteChanged();
        }

        public void UpdateBufferCommands()
        {
            HideBufferCommand.OnCanExecuteChanged();
            CloseBufferCommand.OnCanExecuteChanged();
        }

        public void SaveWindowSize()
        {
            if (_relayConfiguration.HasPropertyChanged(nameof(_relayConfiguration.WindowWidth)) ||
                _relayConfiguration.HasPropertyChanged(nameof(_relayConfiguration.WindowHeight)))
                ConfigurationHelper.SaveConfiguration(_relayConfiguration);
        }

        private void SetStatusText(string message)
        {
            ConnectionStatus = $"[{DateTime.Now:HH:mm:ss}] {message}";
            NotifyPropertyChanged(nameof(ConnectionStatus));
        }
    }
}
