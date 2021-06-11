using System;
using System.Windows;
using System.Windows.Controls;
using MWindowLib;
using WinWeelay.Configuration;
using WinWeelay.Utils;

namespace WinWeelay
{
    /// <summary>
    /// Window to edit the relay's settings.
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        /// <summary>
        /// The configuration to edit.
        /// </summary>
        public RelayConfiguration Configuration
        {
            get
            {
                return ((SettingsViewModel)DataContext).Configuration;
            }
        }

        /// <summary>
        /// Create a new instance of the window.
        /// </summary>
        /// <param name="settingsViewModel">View model to handle the logic.</param>
        public SettingsWindow(SettingsViewModel settingsViewModel)
        {
            InitializeComponent();
            DataContext = settingsViewModel;
            _passwordBox.Password = Cipher.Decrypt(settingsViewModel.Configuration.RelayPassword);
            settingsViewModel.Configuration.StartTrackingChanges();

            _fontComboBox.ItemsSource = FontUtils.GetInstalledFonts();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            RelayConfiguration relayConfiguration = ((SettingsViewModel)DataContext).Configuration;
            relayConfiguration.RelayPassword = Cipher.Encrypt(_passwordBox.Password);

            if (relayConfiguration.HasChanges())
            {
                ConfigurationHelper.SaveConfiguration(relayConfiguration);
                DialogResult = true;
            }
            else
                DialogResult = false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void AccentColor_ValueChanged(object sender, EventArgs e)
        {
            ((SettingsViewModel)DataContext).NotifyAccentColorChanged();
        }

        private void ConnectionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((SettingsViewModel)DataContext).NotifySocketPathVisibleChanged();
        }

        private void HandshakeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((SettingsViewModel)DataContext).NotifyHandshakeTypeChanged();
        }

        private void NotificationsCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            ((SettingsViewModel)DataContext).NotifyNotificationsEnabledChanged();
        }

        private void OptionCacheCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            ((SettingsViewModel)DataContext).NotifyOptionCacheEnabledChanged();
        }

        private void SpellCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            ((SettingsViewModel)DataContext).NotifySpellCheckerEnabledChanged();
        }

        private void InstallDictionaryButton_Click(object sender, RoutedEventArgs e)
        {
            ((SettingsViewModel)DataContext).InstallDictionary();
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((SettingsViewModel)DataContext).NotifySpellCheckerEnabledChanged();
        }
    }
}
