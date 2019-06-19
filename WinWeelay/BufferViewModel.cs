using System;
using WinWeelay.Configuration;
using WinWeelay.Core;
using WinWeelay.Utils;

namespace WinWeelay
{
    public class BufferViewModel
    {
        private MainWindow _mainWindow;
        private RelayConfiguration _relayConfiguration;

        public RelayConnection Connection { get; private set; }

        public DelegateCommand ConnectCommand { get; private set; }
        public DelegateCommand DisconnectCommand { get; private set; }
        public DelegateCommand HideBufferCommand { get; private set; }
        public DelegateCommand CloseBufferCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand SettingsCommand { get; private set; }

        public BufferViewModel() { }

        public BufferViewModel(MainWindow window)
        {
            _mainWindow = window;

            _relayConfiguration = ConfigurationHelper.LoadConfiguration();
            Connection = new RelayConnection(window, _relayConfiguration);

            ConnectCommand = new DelegateCommand(Connect, CanConnect);
            DisconnectCommand = new DelegateCommand(Disconnect, CanDisconnect);
            HideBufferCommand = new DelegateCommand(HideBuffer, CanCloseHide);
            CloseBufferCommand = new DelegateCommand(CloseBuffer, CanCloseHide);
            ExitCommand = new DelegateCommand(Exit);
            SettingsCommand = new DelegateCommand(ShowSettingsForm);

            if (_relayConfiguration.AutoConnect)
                Connect(null);
        }

        private void ShowSettingsForm(object parameter)
        {
            RelayConfiguration config = CloneHelper.DeepCopy(_relayConfiguration);
            SettingsWindow settingsWindow = new SettingsWindow(config);
            if (settingsWindow.ShowDialog() == true)
            {
                _relayConfiguration = settingsWindow.Configuration;

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
            Connection.Connect();
            UpdateConnectionCommands();
        }

        private bool CanDisconnect(object parameter)
        {
            return Connection.IsConnected;
        }

        private void Disconnect(object parameter)
        {
            if (CanDisconnect(parameter))
                Connection.Disconnect();
            UpdateConnectionCommands();
        }

        private void UpdateConnectionCommands()
        {
            ConnectCommand.OnCanExecuteChanged();
            DisconnectCommand.OnCanExecuteChanged();
        }

        public void UpdateBufferCommands()
        {
            HideBufferCommand.OnCanExecuteChanged();
            CloseBufferCommand.OnCanExecuteChanged();
        }
    }
}
