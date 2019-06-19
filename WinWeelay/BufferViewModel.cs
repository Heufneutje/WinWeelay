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

        private bool CanStopConnecting(object parameter)
        {
            return _isRetryingConnection;
        }

        private void StopConnecting(object parameter)
        {
            _isRetryingConnection = true;
            _retryTimer.Stop();
        }

        private void Connection_ConnectionLost(object sender, ConnectionLostEventArgs args)
        {
            _mainWindow.Dispatcher.Invoke(() => 
            {
                bool wasConnected = Connection.IsConnected || _isRetryingConnection;

                if (wasConnected)
                    SetStatusText($"Connection lost, reason: {args.Error.Message}. Retrying in 10 seconds...");
                else
                    SetStatusText($"Connection failed, reason: {args.Error.Message}.");

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
            RelayConfiguration config = CloneHelper.DeepCopy(_relayConfiguration);
            SettingsWindow settingsWindow = new SettingsWindow(config);
            if (settingsWindow.ShowDialog() == true)
            {
                _relayConfiguration = settingsWindow.Configuration;
                Connection.Configuration = _relayConfiguration;

                if (Connection.IsConnected)
                {
                    Disconnect(parameter);
                    Connect(parameter);
                }
            }
        }

        private void Exit(object parameter)
        {
            _mainWindow.Close();
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

        private bool CanConnect(object parameter)
        {
            return !Connection.IsConnected;
        }

        private void Connect(object parameter)
        {
            SetStatusText($"Attempting to connect to {_relayConfiguration.ConnectionAddress}...");
            if (Connection.Connect())
            {
                _isRetryingConnection = false;
                UpdateConnectionCommands();
                SetStatusText($"Connected to {_relayConfiguration.ConnectionAddress}.");
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

        private void SetStatusText(string message)
        {
            ConnectionStatus = $"[{DateTime.Now:HH:mm:ss}] {message}";
            NotifyPropertyChanged(nameof(ConnectionStatus));
        }
    }
}
