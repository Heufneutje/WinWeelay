using System.Windows;
using WinWeelay.Configuration;
using WinWeelay.Utils;

namespace WinWeelay
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public RelayConfiguration Configuration
        {
            get
            {
                return (RelayConfiguration)DataContext;
            }
        }

        public SettingsWindow(RelayConfiguration relayConfiguration)
        {
            InitializeComponent();
            DataContext = relayConfiguration;
            _passwordBox.Password = Cipher.Decrypt(relayConfiguration.RelayPassword);
            relayConfiguration.StartTrackingChanges();

            _fontComboBox.ItemsSource = FormattingHelper.GetInstalledFonts();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            RelayConfiguration relayConfiguration = (RelayConfiguration)DataContext;
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
    }
}
