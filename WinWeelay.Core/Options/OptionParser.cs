using System;
using System.Collections.Generic;
using System.Linq;
using WinWeelay.Configuration;

namespace WinWeelay.Core
{
    public class OptionParser
    {
        private Dictionary<int, string> _colorOptions;
        private Dictionary<string, int> _reverseColorOptions;
        private OptionCache _optionCache;
        private RelayConfiguration _relayConfiguration;
        private List<RelayOption> _weechatOptions;

        public bool HasOptionCache => _optionCache.HasOptions;

        public OptionParser(RelayConfiguration relayConfiguration)
        {
            _relayConfiguration = relayConfiguration;
            _optionCache = OptionCache.Load(relayConfiguration);
            _weechatOptions = new List<RelayOption>();
            _colorOptions = new Dictionary<int, string>
            {
                { 0, OptionNames.WeechatColorSeparator },
                { 1, OptionNames.WeechatColorChat },
                { 2, OptionNames.WeechatColorChatTime },
                { 3, OptionNames.WeechatColorChatTImeDelimiters },
                { 4, OptionNames.WeechatColorChatPrefixError },
                { 5, OptionNames.WeechatColorChatPrefixNetwork },
                { 6, OptionNames.WeechatColorChatPrefixAction },
                { 7, OptionNames.WeechatColorChatPrefixJoin },
                { 8, OptionNames.WeechatColorChatPrefixQuit },
                { 9, OptionNames.WeechatColorChatPrefixMore },
                { 10, OptionNames.WeechatColorChatPrefixSuffix },
                { 11, OptionNames.WeechatColorChatBuffer },
                { 12, OptionNames.WeechatColorChatServer },
                { 13, OptionNames.WeechatColorChatChannel },
                { 14, OptionNames.WeechatColorChatNick },
                { 15, OptionNames.WeechatColorChatNickSelf },
                { 16, OptionNames.WeechatColorChatNickOther },
                { 27, OptionNames.WeechatColorChatHost }, // 17 through 26 are no longer in use.
                { 28, OptionNames.WeechatColorChatDelimiters },
                { 29, OptionNames.WeechatColorChatHighlight},
                { 30, OptionNames.WeechatColorChatReadMarker },
                { 31, OptionNames.WeechatColorChatTextFound },
                { 32, OptionNames.WeechatColorChatValue },
                { 33, OptionNames.WeechatColorChatPrefixBuffer },
                { 34, OptionNames.WeechatColorChatTags },
                { 35, OptionNames.WeechatColorChatInactiveWindow },
                { 36, OptionNames.WeechatColorChatInactiveBuffer },
                { 37, OptionNames.WeechatColorChatPrefixBufferInactiveBuffer },
                { 38, OptionNames.WeechatColorChatNickOffline },
                { 39, OptionNames.WeechatColorChatNickOfflineHighlight },
                { 40, OptionNames.WeechatColorChatNickPrefix },
                { 41, OptionNames.WeechatColorChatNickSuffix },
                { 42, OptionNames.WeechatColorEmphasized },
                { 43, OptionNames.WeechatColorChatDayChange },
                { 44, OptionNames.WeechatColorChatValueNull }
            };

            _reverseColorOptions = new Dictionary<string, int>();
            foreach (KeyValuePair<int, string> pair in _colorOptions)
                _reverseColorOptions.Add(pair.Value, pair.Key);
        }

        public void ParseOptionsCached(WeechatInfoList optionsList)
        {
            _optionCache.Options.Clear();

            foreach (Dictionary<string, WeechatRelayObject> item in optionsList)
                _optionCache.Options.Add(new RelayOption(item["full_name"].AsString(), item["value"].AsString()));
        }

        public void ParseOptionsUncached(WeechatInfoList optionsList)
        {
            _weechatOptions.Clear();
            foreach (Dictionary<string, WeechatRelayObject> item in optionsList.Items)
                _weechatOptions.Add(new RelayOption(item));
        }

        public List<RelayOption> GetParsedOptions()
        {
            return _weechatOptions.ToList();
        }

        public void SaveOptionCache()
        {
            _optionCache.SaveOptionCache(_relayConfiguration);
        }

        public int GetOptionColor(int colorCode)
        {
            if (!_colorOptions.ContainsKey(colorCode))
                return (int)WeechatColor.Default;

            RelayOption option = _optionCache.Options.FirstOrDefault(x => x.Name == _colorOptions[colorCode]);
            if (option == null)
                return (int)WeechatColor.Default;

            //string value = option.Value.ToLower().Replace("_", string.Empty).Replace("*", string.Empty);
            string value = option.Value;
            if (value.StartsWith("color"))
                return Convert.ToInt32(value.Replace("color", string.Empty));

            bool result = Enum.TryParse(value.Replace("_", string.Empty).Replace("*", string.Empty), true, out WeechatColor color);
            return result ? (int)color : -1;
        }

        public string GetOptionColorCode(string option)
        {
            if (!_reverseColorOptions.ContainsKey(option))
                return "00";

            return _reverseColorOptions[option].ToString().PadLeft(2, '0');
        }
    }
}
