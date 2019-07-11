﻿using System;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Timers;
using WinWeelay.Configuration;
using WinWeelay.Core;
using WinWeelay.Properties;
using WinWeelay.Utils;
using System.Windows;

#if WINDOWS10_SDK
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
#endif

namespace WinWeelay
{
    public class BufferViewModel : NotifyPropertyChangedBase
    {
        private MainWindow _mainWindow;
        private RelayConfiguration _relayConfiguration;
        private Timer _retryTimer;
        private bool _isRetryingConnection;
        private ThemeManager _themeManager;
        private bool _isDownloadingUpdate;
        
        public RelayConnection Connection { get; private set; }
        public string ConnectionStatus { get; set; }

        public DelegateCommand ConnectCommand { get; private set; }
        public DelegateCommand DisconnectCommand { get; private set; }
        public DelegateCommand HideBufferCommand { get; private set; }
        public DelegateCommand CloseBufferCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand SettingsCommand { get; private set; }
        public DelegateCommand StopConnectingCommand { get; private set; }
        public DelegateCommand AboutCommand { get; private set; }
        public DelegateCommand WhoisCommand { get; private set; }
        public DelegateCommand QueryCommand { get; private set; }
        public DelegateCommand KickCommand { get; private set; }
        public DelegateCommand BanCommand { get; private set; }
        public DelegateCommand KickbanCommand { get; private set; }
        public DelegateCommand LoadMoreMessagesCommand { get; private set; }
        public DelegateCommand SourceCodeCommand { get; private set; }
        public DelegateCommand IssueTrackerCommand { get; private set; }
        public DelegateCommand CheckForUpdateCommand { get; private set; }

        public BufferViewModel() { }

        public BufferViewModel(MainWindow window)
        {
            _mainWindow = window;

            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinWeelay");
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            _retryTimer = new Timer(10000);
            _retryTimer.Elapsed += RetryTimer_Elapsed;

            _relayConfiguration = ConfigurationHelper.LoadConfiguration();
            Connection = new RelayConnection(window, _relayConfiguration);
            _mainWindow.ToggleSpellChecker(_relayConfiguration.IsSpellCheckEnabled);

            SetStatusText("Disconnected.");

            _themeManager = new ThemeManager();
            _themeManager.InitializeThemes(_relayConfiguration);

            ConnectCommand = new DelegateCommand(Connect, CanConnect);
            DisconnectCommand = new DelegateCommand(Disconnect, CanDisconnect);
            HideBufferCommand = new DelegateCommand(HideBuffer, IsBufferSelected);
            CloseBufferCommand = new DelegateCommand(CloseBuffer, IsBufferSelected);
            ExitCommand = new DelegateCommand(Exit);
            SettingsCommand = new DelegateCommand(ShowSettingsWindow);
            StopConnectingCommand = new DelegateCommand(StopConnecting, CanStopConnecting);
            AboutCommand = new DelegateCommand(ShowAboutWindow);
            WhoisCommand = new DelegateCommand(SendWhois, IsNickSelected);
            QueryCommand = new DelegateCommand(OpenQuery, IsNickSelected);
            KickCommand = new DelegateCommand(SendKick, IsNickSelected);
            BanCommand = new DelegateCommand(SendBan, IsNickSelected);
            KickbanCommand = new DelegateCommand(SendKickban, IsNickSelected);
            LoadMoreMessagesCommand = new DelegateCommand(LoadMoreMessages, IsBufferSelected);
            SourceCodeCommand = new DelegateCommand(OpenSourceCode);
            IssueTrackerCommand = new DelegateCommand(OpenIssueTracker);
            CheckForUpdateCommand = new DelegateCommand(HandleCheckForUpdate, CanCheckForUpdate);

            Connection.ConnectionLost += Connection_ConnectionLost;
            Connection.Highlighted += Connection_Highlighted;

            if (_relayConfiguration.AutoCheckUpdates)
                CheckForUpdate(false);

            if (_relayConfiguration.AutoConnect)
                Connect(null);
        }

        private void Connection_Highlighted(object sender, HighlightEventArgs args)
        {
            args.Message.IsNotified = true;

#if WINDOWS10_SDK
            if (args.Buffer != Connection.ActiveBuffer || !_mainWindow.IsActive)
            {
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);
                XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                stringElements[0].AppendChild(toastXml.CreateTextNode(args.Buffer.Name));
                stringElements[1].AppendChild(toastXml.CreateTextNode(args.Message.UnformattedMessage));

                string tempPath = Path.Combine(Path.GetTempPath(), "weechat.png");
                if (!File.Exists(tempPath))
                    Resources.weechat.Save(tempPath);

                XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                imageElements[0].Attributes.GetNamedItem("src").NodeValue = tempPath;

                ToastNotification toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier("WinWeelay").Show(toast);
            }
#endif
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
                SetStatusConnected();
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

        private bool IsBufferSelected(object parameter)
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

        private void ShowSettingsWindow(object parameter)
        {
            RelayConfiguration config = new RelayConfiguration();
            _relayConfiguration.CopyPropertiesTo(config);

            SettingsWindow settingsWindow = new SettingsWindow(config) { Owner = _mainWindow };
            if (settingsWindow.ShowDialog() == true)
            {
                _relayConfiguration = settingsWindow.Configuration;
                Connection.Configuration = _relayConfiguration;

                if (config.HasPropertyChanged(nameof(config.Theme)))
                    _themeManager.UpdateTheme(config.Theme);

                if (config.HasPropertyChanged(nameof(config.FontFamily)) || config.HasPropertyChanged(nameof(config.FontSize)))
                    _mainWindow.UpdateFont();

                if (config.HasPropertyChanged(nameof(config.IsSpellCheckEnabled)))
                    _mainWindow.ToggleSpellChecker(_relayConfiguration.IsSpellCheckEnabled);

                _relayConfiguration.ResetTrackingChanges();
                _relayConfiguration.StartTrackingChanges();
            }
        }

        private void ShowAboutWindow(object parameter)
        {
            AboutWindow aboutWindow = new AboutWindow() { Owner = _mainWindow };
            aboutWindow.ShowDialog();
        }

        private bool IsNickSelected(object parameter)
        {
            return Connection.ActiveBuffer?.ActiveNicklistEntry != null;
        }

        private void SendWhois(object parameter)
        {
            Connection.ActiveBuffer?.SendWhois();
        }

        private void OpenQuery(object parameter)
        {
            if (Connection.ActiveBuffer != null && Connection.ActiveBuffer.ActiveNicklistEntry != null)
                Connection.OutputHandler.Input(Connection.ActiveBuffer, $"/query {Connection.ActiveBuffer?.ActiveNicklistEntry?.Name}");
        }

        private void SendKick(object parameter)
        {
            Connection.ActiveBuffer?.SendKick();
        }

        private void SendBan(object parameter)
        {
            Connection.ActiveBuffer?.SendBan();
        }

        private void SendKickban(object parameter)
        {
            Connection.ActiveBuffer?.SendKickban();
        }

        private void LoadMoreMessages(object parameter)
        {
            Connection.ActiveBuffer?.LoadMoreMessages();
        }

        private void OpenSourceCode(object parameter)
        {
            Process.Start("https://github.com/Heufneutje/WinWeelay");
        }

        private void OpenIssueTracker(object parameter)
        {
            Process.Start("https://github.com/Heufneutje/WinWeelay/issues");
        }

        private void HandleCheckForUpdate(object parameter)
        {
            CheckForUpdate(true);
        }

        private bool CanCheckForUpdate(object parameter)
        {
            return !_isDownloadingUpdate;
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
            LoadMoreMessagesCommand.OnCanExecuteChanged();
        }

        public void UpdateActiveNicklistEntry(RelayNicklistEntry nick)
        {
            if (Connection.ActiveBuffer != null)
            {
                Connection.ActiveBuffer.ActiveNicklistEntry = nick;
                WhoisCommand.OnCanExecuteChanged();
                QueryCommand.OnCanExecuteChanged();
                KickCommand.OnCanExecuteChanged();
                BanCommand.OnCanExecuteChanged();
                KickbanCommand.OnCanExecuteChanged();
            }
        }

        public void SaveWindowSize()
        {
            if (_relayConfiguration.HasPropertyChanged(nameof(_relayConfiguration.WindowWidth)) ||
                _relayConfiguration.HasPropertyChanged(nameof(_relayConfiguration.WindowHeight)))
                ConfigurationHelper.SaveConfiguration(_relayConfiguration);
        }

        private void SetStatusText(string message)
        {
            ConnectionStatus = $"[{DateTime.Now.ToString(_relayConfiguration.TimestampFormat)}] {message}";
            NotifyPropertyChanged(nameof(ConnectionStatus));
        }

        private void SetStatusConnected()
        {
            SetStatusText($"Connected to {_relayConfiguration.ConnectionAddress}{(_relayConfiguration.ConnectionType == RelayConnectionType.WeechatSsl ? " (secure connection)" : "")}.");
        }

        private void CheckForUpdate(bool shouldPopUp)
        {
            _isDownloadingUpdate = true;
            CheckForUpdateCommand.OnCanExecuteChanged();

            SetStatusText("Checking for updates...");

            BackgroundWorker updateCheckerBackgroundWorker = new BackgroundWorker();
            UpdateHelper updateHelper = new UpdateHelper();
            updateCheckerBackgroundWorker.DoWork += (sender, args) =>
            {
                args.Result = updateHelper.CheckForUpdate();
            };
            updateCheckerBackgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                UpdateCheckResult result = (UpdateCheckResult)args.Result;
                if (result.ResultType == UpdateResultType.UpdateAvailable)
                {
                    if (ThemedMessageBoxWindow.Show(result.Message, result.MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        DownloadUpdate(updateHelper, result.DownloadUrl);
                        return;
                    }
                }
                else
                {
                    if (shouldPopUp)
                        ThemedMessageBoxWindow.Show(result.Message, result.MessageTitle, MessageBoxButton.OK, result.ResultType == UpdateResultType.NoUpdateAvailable ? MessageBoxImage.Information : MessageBoxImage.Error);
                }

                FinishUpdateCheck();
                updateHelper.Dispose();
            };
            updateCheckerBackgroundWorker.RunWorkerAsync();
        }

        private void DownloadUpdate(UpdateHelper updateHelper, string downloadUrl)
        {
            SetStatusText("Downloading update files...");
            _mainWindow.SetProgressBarVisibility(true);

            updateHelper.ProgressChanged += (sender, args) =>
            {
                _mainWindow.SetProgress(args.ProgressPercentage);
            };
            updateHelper.DownloadCompleted += (sender, args) =>
            {
                if (args.Error != null)
                {
                    ThemedMessageBoxWindow.Show($"An error occurred while running the update installer:{Environment.NewLine}{args.Error.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    try
                    {
                        ProcessStartInfo psi = new ProcessStartInfo(updateHelper.InstallerFilePath);
                        psi.Arguments = $"/S /UPDATELOCATION=\"{updateHelper.GetApplicationDirectory()}\"";
                        updateHelper.Dispose();
                        Process.Start(psi);
                        Application.Current.Shutdown();
                    }
                    catch (Exception ex)
                    {
                        ThemedMessageBoxWindow.Show($"An error occurred while running the update installer:{Environment.NewLine}{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        updateHelper.Dispose();
                        FinishUpdateCheck();
                    }
                }
            };
            updateHelper.DownloadUpdate(downloadUrl);
        }

        private void FinishUpdateCheck()
        {
            _isDownloadingUpdate = false;
            CheckForUpdateCommand.OnCanExecuteChanged();
            _mainWindow.SetProgressBarVisibility(false);

            if (Connection.IsConnected)
                SetStatusConnected();
            else
                SetStatusText("Disconnected.");
        }
    }
}
