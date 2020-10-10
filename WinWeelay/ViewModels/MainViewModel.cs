﻿using System;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Timers;
using WinWeelay.Configuration;
using WinWeelay.Core;
using WinWeelay.Utils;
using System.Windows;

#if WINDOWS10_SDK
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using WinWeelay.Properties;
#endif

namespace WinWeelay
{
    /// <summary>
    /// The main view model for the application.
    /// </summary>
    public class MainViewModel : NotifyPropertyChangedBase
    {
        private MainWindow _mainWindow;
        private OptionsListWindow _optionsListWindow;
        private Timer _retryTimer;
        private bool _isRetryingConnection;
        private ThemeManager _themeManager;
        private SpellingManager _spellingManager;
        private bool _isDownloadingUpdate;

        /// <summary>
        /// The main configuration loaded from the config file.
        /// </summary>
        public RelayConfiguration RelayConfiguration { get; set; }

        /// <summary>
        /// Connection to the WeeChat host.
        /// </summary>
        public RelayConnection Connection { get; private set; }

        /// <summary>
        /// The text to display in the status bar.
        /// </summary>
        public string ConnectionStatus { get; set; }

        /// <summary>
        /// Command to connect to a WeeChat host.
        /// </summary>
        public DelegateCommand ConnectCommand { get; private set; }

        /// <summary>
        /// Command to disconnect from a WeeChat host.
        /// </summary>
        public DelegateCommand DisconnectCommand { get; private set; }

        /// <summary>
        /// Command to close a buffer tab.
        /// </summary>
        public DelegateCommand HideBufferCommand { get; private set; }

        /// <summary>
        /// Command to close a buffer on the host.
        /// </summary>
        public DelegateCommand CloseBufferCommand { get; private set; }

        /// <summary>
        /// Command to close the application.
        /// </summary>
        public DelegateCommand ExitCommand { get; private set; }

        /// <summary>
        /// Command to display the settings dialog.
        /// </summary>
        public DelegateCommand SettingsCommand { get; private set; }

        /// <summary>
        /// Command to display the WeeChat option editor.
        /// </summary>
        public DelegateCommand WeeChatOptionsCommand { get; private set; }

        /// <summary>
        /// Command to display the about dialog.
        /// </summary>
        public DelegateCommand AboutCommand { get; private set; }

        /// <summary>
        /// Command to use a /WHOIS command on the currently selected user.
        /// </summary>
        public DelegateCommand WhoisCommand { get; private set; }

        /// <summary>
        /// Command to open a query buffer for the currently selected user.
        /// </summary>
        public DelegateCommand QueryCommand { get; private set; }

        /// <summary>
        /// Command to kick the currently selected user from the channel.
        /// </summary>
        public DelegateCommand KickCommand { get; private set; }

        /// <summary>
        /// Command to ban the currently selected user from the channel.
        /// </summary>
        public DelegateCommand BanCommand { get; private set; }

        /// <summary>
        /// Command to kick and ban the currently selected user from the channel.
        /// </summary>
        public DelegateCommand KickbanCommand { get; private set; }

        /// <summary>
        /// Command to load more messages from the currently selected buffer's backlog.
        /// </summary>
        public DelegateCommand LoadMoreMessagesCommand { get; private set; }

        /// <summary>
        /// Command to open the GitHub repository in the default browser.
        /// </summary>
        public DelegateCommand SourceCodeCommand { get; private set; }

        /// <summary>
        /// Command to open the GitHub issues tracker in the default browser.
        /// </summary>
        public DelegateCommand IssueTrackerCommand { get; private set; }

        /// <summary>
        /// Command to check whether a new version is available.
        /// </summary>
        public DelegateCommand CheckForUpdateCommand { get; private set; }

        /// <summary>
        /// Command to clear all messages from the active buffer on the host.
        /// </summary>
        public DelegateCommand ClearBufferCommand { get; private set; }

        /// <summary>
        /// Command to reset the buffer size back to its default value and reload the messages in it.
        /// </summary>
        public DelegateCommand ReinitBufferCommand { get; private set; }

        /// <summary>
        /// Empty constructor for the designer.
        /// </summary>
        public MainViewModel() { }

        /// <summary>
        /// Create a new view model for the main window.
        /// </summary>
        /// <param name="window">The main window.</param>
        public MainViewModel(MainWindow window)
        {
            _mainWindow = window;
            _mainWindow.Closed += MainWindow_Closed;

            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WinWeelay");
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            _retryTimer = new Timer(10000);
            _retryTimer.Elapsed += RetryTimer_Elapsed;

            RelayConfiguration = ConfigurationHelper.LoadConfiguration();
            Connection = new RelayConnection(window, RelayConfiguration);
            SetBufferListType();

            SetStatusText("Disconnected.");

            _themeManager = new ThemeManager();
            _themeManager.InitializeThemes(RelayConfiguration);

            System.Threading.Thread.CurrentThread.CurrentUICulture = RelayConfiguration.Language;
            _spellingManager = new SpellingManager();
            _mainWindow.SetSpellingManager(_spellingManager);
            _mainWindow.ToggleSpellChecker(RelayConfiguration.IsSpellCheckEnabled);

            ConnectCommand = new DelegateCommand(Connect, CanConnect);
            DisconnectCommand = new DelegateCommand(Disconnect, CanDisconnect);
            HideBufferCommand = new DelegateCommand(HideBuffer, IsBufferSelected);
            CloseBufferCommand = new DelegateCommand(CloseBuffer, IsBufferSelected);
            ExitCommand = new DelegateCommand(Exit);
            SettingsCommand = new DelegateCommand(ShowSettingsWindow);
            WeeChatOptionsCommand = new DelegateCommand(ShowWeeChatOptionsWindow, CanDisconnect);
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
            ClearBufferCommand = new DelegateCommand(ClearBuffer, IsBufferSelected);
            ReinitBufferCommand = new DelegateCommand(ReinitBuffer, IsBufferSelected);

            Connection.ConnectionLost += Connection_ConnectionLost;
            Connection.Highlighted += Connection_Highlighted;

            if (RelayConfiguration.AutoCheckUpdates)
                CheckForUpdate(false);

            if (RelayConfiguration.AutoConnect)
                Connect(null);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (_optionsListWindow != null)
                _optionsListWindow.Close();
        }

        private void Connection_Highlighted(object sender, HighlightEventArgs args)
        {
            args.Message.IsNotified = true;

            #if WINDOWS10_SDK
            if (RelayConfiguration.NotificationsEnabled && (RelayConfiguration.NotificationsEnabledWithBufferFocus || args.Buffer != Connection.ActiveBuffer || !_mainWindow.IsActive || _mainWindow.WindowState == WindowState.Minimized))
            {
                try
                {
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);
                    XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                    stringElements[0].AppendChild(toastXml.CreateTextNode(args.Buffer.FullName));
                    stringElements[1].AppendChild(toastXml.CreateTextNode(args.Message.UnformattedMessage));

                    string tempPath = Path.Combine(Path.GetTempPath(), "weechat.png");
                    if (!File.Exists(tempPath))
                        Resources.weechat.Save(tempPath);

                    XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                    imageElements[0].Attributes.GetNamedItem("src").NodeValue = tempPath;

                    ToastNotification toast = new ToastNotification(toastXml);
                    ToastNotificationManager.CreateToastNotifier("WinWeelay").Show(toast);
                }
                catch (Exception)
                {
                    // Ignore the error as it likely means notifications simply aren't available.
                }
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
            if (string.IsNullOrEmpty(RelayConfiguration.Hostname) || RelayConfiguration.Port == 0)
            {
                SetStatusText($"Connection failed, reason: configuration is invalid.");
                return;
            }

            UpdateConnectionCommands();
            SetStatusText($"Attempting to connect to {RelayConfiguration.ConnectionAddress}...");

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
            return Connection.IsConnected || _isRetryingConnection;
        }

        private void Disconnect(object parameter)
        {
            if (Connection.IsConnected)
                Connection.Disconnect(true);
            else
            {
                _isRetryingConnection = false;
                _retryTimer.Stop();
            }
            UpdateConnectionCommands();
            SetStatusText($"Disconnected.");
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
            SettingsWindow settingsWindow = new SettingsWindow(new SettingsViewModel(RelayConfiguration, _spellingManager)) { Owner = _mainWindow };
            if (settingsWindow.ShowDialog() == true)
            {
                if (RelayConfiguration.HasPropertyChanged(nameof(RelayConfiguration.Theme)) || RelayConfiguration.AccentColor.HasChanges())
                    _themeManager.UpdateTheme(RelayConfiguration.Theme, RelayConfiguration.AccentColor);

                if (RelayConfiguration.HasPropertyChanged(nameof(RelayConfiguration.FontFamily)) || RelayConfiguration.HasPropertyChanged(nameof(RelayConfiguration.FontSize)))
                    _mainWindow.UpdateFont();

                if (RelayConfiguration.HasPropertyChanged(nameof(RelayConfiguration.Language)))
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = RelayConfiguration.Language;
                    _spellingManager.SetDictionaryPaths();
                }
                    
                if (RelayConfiguration.HasPropertyChanged(nameof(RelayConfiguration.IsSpellCheckEnabled)))
                    _mainWindow.ToggleSpellChecker(RelayConfiguration.IsSpellCheckEnabled);

                if (RelayConfiguration.HasPropertyChanged(nameof(RelayConfiguration.BufferViewType)))
                    SetBufferListType();

                if (RelayConfiguration.HasPropertyChanged(nameof(RelayConfiguration.IsMessageFormattingEnabled)))
                {
                    _mainWindow.UpdateFormattingSettings();
                    foreach (RelayBuffer buffer in Connection.Buffers)
                        buffer.ReinitMessages();
                }
            }
            else
            {
                RelayConfiguration.UndoChanges();
            }

            RelayConfiguration.ResetTrackingChanges();
            RelayConfiguration.StartTrackingChanges();
        }

        private void ShowWeeChatOptionsWindow(object parameter)
        {
            if (_optionsListWindow == null)
            {
                OptionsListViewModel viewModel = new OptionsListViewModel(Connection);
                _optionsListWindow = new OptionsListWindow(viewModel);
                _optionsListWindow.Closed += OptionsListWindow_Closed;

                viewModel.Owner = _optionsListWindow;
                _optionsListWindow.Show();
            }
            _optionsListWindow.Activate();
        }

        private void OptionsListWindow_Closed(object sender, EventArgs e)
        {
            _optionsListWindow = null;
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

        private void ClearBuffer(object parameter)
        {
            Connection.ActiveBuffer?.SendClear();
        }

        private void ReinitBuffer(object parameter)
        {
            Connection.ActiveBuffer?.ReinitMessages();
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
            WeeChatOptionsCommand.OnCanExecuteChanged();
        }

        /// <summary>
        /// Update the state of all commands which interact with buffers.
        /// </summary>
        public void UpdateBufferCommands()
        {
            HideBufferCommand.OnCanExecuteChanged();
            CloseBufferCommand.OnCanExecuteChanged();
            LoadMoreMessagesCommand.OnCanExecuteChanged();
            ClearBufferCommand.OnCanExecuteChanged();
            ReinitBufferCommand.OnCanExecuteChanged();
        }

        /// <summary>
        /// Update the state of all commands which interact with the currently selected user.
        /// </summary>
        /// <param name="nick">The currently selected user.</param>
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

        /// <summary>
        /// Save the current window size to the configuration file.
        /// </summary>
        public void SaveWindowSize()
        {
            if (RelayConfiguration.HasPropertyChanged(nameof(RelayConfiguration.WindowWidth)) ||
                RelayConfiguration.HasPropertyChanged(nameof(RelayConfiguration.WindowHeight)))
                ConfigurationHelper.SaveConfiguration(RelayConfiguration);
        }

        /// <summary>
        /// Save bar visibility changes to the configuration file.
        /// </summary>
        public void UpdateViewSettings()
        {
            if (RelayConfiguration.HasChanges())
            {
                ConfigurationHelper.SaveConfiguration(RelayConfiguration);
                if (RelayConfiguration.HasPropertyChanged(nameof(RelayConfiguration.IsToolbarVisible)) || RelayConfiguration.HasPropertyChanged(nameof(RelayConfiguration.IsStatusBarVisible)))
                    RelayConfiguration.NotifyViewPropertiesChanged();

                RelayConfiguration.ResetTrackingChanges();
                RelayConfiguration.StartTrackingChanges();
            }
        }

        /// <summary>
        /// Save the cached WeeChat options.
        /// </summary>
        public void SaveOptionCache()
        {
            Connection.OptionParser.SaveOptionCache();
        }

        private void SetStatusText(string message)
        {
            ConnectionStatus = $"[{DateTime.Now.ToString(RelayConfiguration.TimestampFormat)}] {message}";
            NotifyPropertyChanged(nameof(ConnectionStatus));
        }

        private void SetStatusConnected()
        {
            SetStatusText($"Connected to {RelayConfiguration.ConnectionAddress}{(RelayConfiguration.ConnectionType == RelayConnectionType.WeechatSsl || RelayConfiguration.ConnectionType == RelayConnectionType.WebSocketSsl ? " (secure connection)" : "")}.");
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
                    if (ThemedMessageBoxWindow.Show(result.Message, result.MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Question, _mainWindow) == MessageBoxResult.Yes)
                    {
                        DownloadUpdate(updateHelper, result.DownloadUrl);
                        return;
                    }
                }
                else
                {
                    if (shouldPopUp)
                        ThemedMessageBoxWindow.Show(result.Message, result.MessageTitle, MessageBoxButton.OK, result.ResultType == UpdateResultType.NoUpdateAvailable ? MessageBoxImage.Information : MessageBoxImage.Error, _mainWindow);
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
                    ThemedMessageBoxWindow.Show($"An error occurred while running the update installer:{Environment.NewLine}{args.Error.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error, _mainWindow);
                }
                else
                {
                    try
                    {
                        ProcessStartInfo psi = new ProcessStartInfo(updateHelper.InstallerFilePath);
                        psi.Arguments = $"/S \"/UPDATELOCATION={updateHelper.GetApplicationDirectory()}\"";
                        updateHelper.Dispose();
                        Process.Start(psi);
                        Application.Current.Shutdown();
                    }
                    catch (Exception ex)
                    {
                        ThemedMessageBoxWindow.Show($"An error occurred while running the update installer:{Environment.NewLine}{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error, _mainWindow);
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

        private void SetBufferListType()
        {
            switch (RelayConfiguration.BufferViewType)
            {
                case BufferViewType.List:
                    _mainWindow.SetBufferControl(new BufferListControl(Connection));
                    break;
                case BufferViewType.Tree:
                    _mainWindow.SetBufferControl(new BufferTreeControl(Connection));
                    break;
            }
        }
    }
}
