using System;
using System.Windows;
using System.Windows.Controls;
using MWindowLib;
using WinWeelay.Configuration;
using WinWeelay.Utils;

namespace WinWeelay
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        public RelayConfiguration Configuration
        {
            get
            {
                return ((SettingsViewModel)DataContext).Configuration;
            }
        }

        public SettingsWindow(SettingsViewModel settingsViewModel)
        {
            InitializeComponent();
            DataContext = settingsViewModel;
            _passwordBox.Password = Cipher.Decrypt(settingsViewModel.Configuration.RelayPassword);
            settingsViewModel.Configuration.StartTrackingChanges();

            _fontComboBox.ItemsSource = FormattingUtils.GetInstalledFonts();
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

        private void _connecetionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((SettingsViewModel)DataContext).NotifySocketPathVisibleChanged();
        }
    }
}
