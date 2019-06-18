using System;
using WinWeelay.Configuration;
using WinWeelay.Core;

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
