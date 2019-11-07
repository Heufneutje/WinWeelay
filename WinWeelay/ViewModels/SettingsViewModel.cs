using Microsoft.Win32;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using WinWeelay.Configuration;
using WinWeelay.Utils;

namespace WinWeelay
{
    public class SettingsViewModel : NotifyPropertyChangedBase
    {
        private SpellingManager _spellingManager;

        public RelayConfiguration Configuration { get; set; }

        public SettingsViewModel()
        {
            _spellingManager = new SpellingManager();
            Configuration = new RelayConfiguration();
        }

        public SettingsViewModel(RelayConfiguration configuration, SpellingManager spellingManager)
        {
            _spellingManager = spellingManager;
            Configuration = configuration;
        }

        public IEnumerable<ConnectionTypeWrapper> ConnectionTypes
        {
            get
            {
                return ConnectionTypeWrapper.GetTypes();
            }
        }

        public IEnumerable<BufferViewTypeWrapper> BufferViewTypes
        {
            get
            {
                return BufferViewTypeWrapper.GetTypes();
            }
        }

        public IEnumerable<CultureInfo> CultureInfos
        {
            get
            {
                return CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).OrderBy(x => x.DisplayName);
            }
        }

        public Color AccentColorExample => Color.FromRgb(Configuration.AccentColor.RedValue, Configuration.AccentColor.GreenValue, Configuration.AccentColor.BlueValue);

        public bool IsWebSocketPathVisible =>  Configuration.ConnectionType == RelayConnectionType.WebSocket || Configuration.ConnectionType == RelayConnectionType.WebSocketSsl;

        public bool NotificationsEnabled => Configuration.NotificationsEnabled;

        public bool UseOptionCache => Configuration.UseOptionCache;

        public bool IsSpellCheckEnabled => Configuration.IsSpellCheckEnabled;

        public bool IsDictionaryInstalled => !IsSpellCheckEnabled || _spellingManager.IsDictionaryInstalled(Configuration.Language);

        public string DictionaryInstalledText
        {
            get
            {
                if (!IsSpellCheckEnabled)
                    return string.Empty;

                return IsDictionaryInstalled ? "Dictionary is installed" : "Dictionary is not installed";
            }
        }

        public void NotifyAccentColorChanged()
        {
            NotifyPropertyChanged(nameof(AccentColorExample));
        }

        public void NotifySocketPathVisibleChanged()
        {
            NotifyPropertyChanged(nameof(IsWebSocketPathVisible));
        }

        public void NotifyNotificationsEnabledChanged()
        {
            NotifyPropertyChanged(nameof(NotificationsEnabled));
        }

        public void NotifyOptionCacheEnabledChanged()
        {
            NotifyPropertyChanged(nameof(UseOptionCache));
        }

        public void NotifySpellCheckerEnabledChanged()
        {
            NotifyPropertyChanged(nameof(IsSpellCheckEnabled));
            NotifyPropertyChanged(nameof(DictionaryInstalledText));
            NotifyPropertyChanged(nameof(IsDictionaryInstalled));
        }

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
