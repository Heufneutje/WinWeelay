using System.Collections.Generic;
using System.Windows.Media;
using WinWeelay.Configuration;
using WinWeelay.Utils;

namespace WinWeelay
{
    public class SettingsViewModel : NotifyPropertyChangedBase
    {
        public RelayConfiguration Configuration { get; set; }

        public SettingsViewModel()
        {
            Configuration = new RelayConfiguration();
        }

        public SettingsViewModel(RelayConfiguration configuration)
        {
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

        public Color AccentColorExample
        {
            get
            {
                return Color.FromRgb(Configuration.AccentColor.RedValue, Configuration.AccentColor.GreenValue, Configuration.AccentColor.BlueValue);
            }
        }

        public bool IsWebSocketPathVisible
        {
            get
            {
                return Configuration.ConnectionType == RelayConnectionType.WebSocket || Configuration.ConnectionType == RelayConnectionType.WebSocketSsl;
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
    }
}
