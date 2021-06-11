using Microsoft.Win32;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using WinWeelay.Configuration;
using WinWeelay.Utils;

namespace WinWeelay
{
    /// <summary>
    /// View model to edit the relay's settings.
    /// </summary>
    public class SettingsViewModel : NotifyPropertyChangedBase
    {
        private SpellingManager _spellingManager;

        /// <summary>
        /// The main configuration loaded from the config file.
        /// </summary>
        public RelayConfiguration Configuration { get; set; }

        /// <summary>
        /// Empty constructor for the designer.
        /// </summary>
        public SettingsViewModel()
        {
            _spellingManager = new SpellingManager();
            Configuration = new RelayConfiguration();
        }

        /// <summary>
        /// Create a new view model to edit the relay's configuration.
        /// </summary>
        /// <param name="configuration">The main configuration loaded from the config file.</param>
        /// <param name="spellingManager">Spell checker to update the settings for.</param>
        public SettingsViewModel(RelayConfiguration configuration, SpellingManager spellingManager)
        {
            _spellingManager = spellingManager;
            Configuration = configuration;
        }

        /// <summary>
        /// Wrapper to display connection types in a combo box.
        /// </summary>
        public IEnumerable<ConnectionTypeWrapper> ConnectionTypes => ConnectionTypeWrapper.GetTypes();

        /// <summary>
        /// Wrapper to display buffer view types in a combo box.
        /// </summary>
        public IEnumerable<BufferViewTypeWrapper> BufferViewTypes => BufferViewTypeWrapper.GetTypes();

        /// <summary>
        /// Wrapper to display handshake types in a combo box.
        /// </summary>
        public IEnumerable<HandshakeTypeWrapper> HandshakeTypes => HandshakeTypeWrapper.GetTypes();

        /// <summary>
        /// List of all languages on the system.
        /// </summary>
        public IEnumerable<CultureInfo> CultureInfos => CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).OrderBy(x => x.DisplayName);

        /// <summary>
        /// Color create from the chosen accent color values.
        /// </summary>
        public Color AccentColorExample => Color.FromRgb(Configuration.AccentColor.RedValue, Configuration.AccentColor.GreenValue, Configuration.AccentColor.BlueValue);

        /// <summary>
        /// Whether to display the input box for the WebSocked path.
        /// </summary>
        public bool IsWebSocketPathVisible =>  Configuration.ConnectionType == RelayConnectionType.WebSocket || Configuration.ConnectionType == RelayConnectionType.WebSocketSsl;

        /// <summary>
        /// Whether notifications are enabled.
        /// </summary>
        public bool NotificationsEnabled => Configuration.NotificationsEnabled;

        /// <summary>
        /// Whether the option cache should be used.
        /// </summary>
        public bool UseOptionCache => Configuration.UseOptionCache;

        /// <summary>
        /// Whether the spell checker is enabled.
        /// </summary>
        public bool IsSpellCheckEnabled => Configuration.IsSpellCheckEnabled;

        /// <summary>
        /// Whether a dictionary is installed for the currently selected language.
        /// </summary>
        public bool IsDictionaryInstalled => !IsSpellCheckEnabled || _spellingManager.IsDictionaryInstalled(Configuration.Language);

        public string HandshakeTypeDescription
        {
            get
            {
                switch (Configuration.HandshakeType)
                {
                    case HandshakeType.Legacy:
                        return "Sends the password in plain text. Required if using WeeChat < 2.9.";
                    case HandshakeType.Modern:
                        return "Sends a hash of the password using the most secure algorithm supported by the relay. WeeChat >= 2.9 is required.";
                    default:
                        return string.Empty;
                }
            }
        }

        /// <summary>
        /// The dictionary info text to display for the currently selected language.
        /// </summary>
        public string DictionaryInstalledText
        {
            get
            {
                if (!IsSpellCheckEnabled)
                    return string.Empty;

                return IsDictionaryInstalled ? "Dictionary is installed" : "Dictionary is not installed";
            }
        }

        /// <summary>
        /// Update the accent color example.
        /// </summary>
        public void NotifyAccentColorChanged()
        {
            NotifyPropertyChanged(nameof(AccentColorExample));
        }

        /// <summary>
        /// Update the visibility of the WebSocket path input box.
        /// </summary>
        public void NotifySocketPathVisibleChanged()
        {
            NotifyPropertyChanged(nameof(IsWebSocketPathVisible));
        }

        /// <summary>
        /// Update the description of the handshake type.
        /// </summary>
        public void NotifyHandshakeTypeChanged()
        {
            NotifyPropertyChanged(nameof(HandshakeTypeDescription));
        }

        /// <summary>
        /// Update the visibility of the notification options.
        /// </summary>
        public void NotifyNotificationsEnabledChanged()
        {
            NotifyPropertyChanged(nameof(NotificationsEnabled));
        }

        /// <summary>
        /// Update the state of the option cache days setting.
        /// </summary>
        public void NotifyOptionCacheEnabledChanged()
        {
            NotifyPropertyChanged(nameof(UseOptionCache));
        }

        /// <summary>
        /// Update the state of the installed dictionary for the currently selected language.
        /// </summary>
        public void NotifySpellCheckerEnabledChanged()
        {
            NotifyPropertyChanged(nameof(IsSpellCheckEnabled));
            NotifyPropertyChanged(nameof(DictionaryInstalledText));
            NotifyPropertyChanged(nameof(IsDictionaryInstalled));
        }

        /// <summary>
        /// Install a dictionary for the currently selected language.
        /// </summary>
        public void InstallDictionary()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { CheckFileExists = true, Filter = "Lexicon files (*.lex)|*.lex|All files (*.*)|*.*", Title = "Select a dictionary" };
            if (openFileDialog.ShowDialog() == true)
            {
                _spellingManager.InstallDictionary(Configuration.Language, openFileDialog.FileName);
                NotifySpellCheckerEnabledChanged();
            }
        }
    }
}
