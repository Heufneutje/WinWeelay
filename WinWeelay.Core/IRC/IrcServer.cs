using System;
using System.Collections.Generic;
using System.Linq;

namespace WinWeelay.Core
{
    /// <summary>
    /// Handler for the properies of a connected IRC server.
    /// </summary>
    public class IrcServer
    {
        /// <summary>
        /// Pointer for the server buffer that relates to the IRC server.
        /// </summary>
        public string BufferPointer { get; private set; }

        /// <summary>
        /// Channel modes that are supported by the IRC server.
        /// </summary>
        public List<IrcMode> AvailableChannelModes { get; private set; }

        /// <summary>
        /// User modes that are supported by the IRC server.
        /// </summary>
        public List<IrcMode> AvailableUserModes { get; private set; }

        /// <summary>
        /// Status modes that are supported by the IRC server.
        /// </summary>
        public List<IrcStatusMode> AvailableStatusModes { get; private set; }

        /// <summary>
        /// Raw ISUPPORT tokens provided by the IRC server.
        /// </summary>
        public Dictionary<string, string> SupportTokens { get; private set; }

        /// <summary>
        /// Our current nickname on the server.
        /// </summary>
        public string CurrentNick { get; set; }

        /// <summary>
        /// The user modes that are currently set.
        /// </summary>
        public string CurrentUserModeString { get; set; }

        /// <summary>
        /// Create a new handler.
        /// </summary>
        public IrcServer()
        {
            AvailableChannelModes = new List<IrcMode>();
            AvailableStatusModes = new List<IrcStatusMode>();
            SupportTokens = new Dictionary<string, string>();
        }

        /// <summary>
        /// Read and parse an infolist response into server properties.
        /// </summary>
        /// <param name="items">Infolist response with IRC server properties.</param>
        public void ParseInfoList(Dictionary<string, WeechatRelayObject> items)
        {
            BufferPointer = items["buffer"].AsPointer();
            CurrentNick = items["nick"].AsString();
            CurrentUserModeString = string.Concat(items["nick_modes"].AsString().OrderBy(c => c));

            ParseSupportTokens(items["isupport"].AsString());
            AvailableChannelModes = ParseSupportedModes(items["chanmodes"].AsString());
            if (SupportTokens.ContainsKey("USERMODES"))
                AvailableUserModes = ParseSupportedModes(SupportTokens["USERMODES"]);

            ParseStatusModes(items["prefix_modes"].AsString(), items["prefix_chars"].AsString());
        }

        /// <summary>
        /// Get the index for sorting the nicklist based on the supported status modes.
        /// </summary>
        /// <param name="prefix">The prefix to check</param>
        /// <returns>The index for sorting the nicklist.</returns>
        public int GetStatusSortIndex(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                return AvailableStatusModes.Count;

            foreach (IrcStatusMode mode in AvailableStatusModes)
                if (prefix.Contains(mode.PrefixChar.ToString()))
                    return AvailableStatusModes.IndexOf(mode);

            return AvailableStatusModes.Count;
        }

        private void ParseSupportTokens(string tokenString)
        {
            SupportTokens.Clear();

            string[] tokenPairs = tokenString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string tokenPair in tokenPairs)
            {
                if (tokenPair.Contains("="))
                {
                    string[] splitPair = tokenPair.Split('=');
                    AddSupportToken(splitPair[0], splitPair[1]);
                }
                else
                    AddSupportToken(tokenPair, null);
            }
        }

        private void AddSupportToken(string key, string value)
        {
            if (!SupportTokens.ContainsKey(key))
                SupportTokens.Add(key, value);
        }

        private void ParseStatusModes(string modeString, string prefixString)
        {
            AvailableStatusModes.Clear();

            for (int i = 0; i < modeString.Length; i++)
                AvailableStatusModes.Add(new IrcStatusMode(modeString[i], prefixString[i]));
        }

        private List<IrcMode> ParseSupportedModes(string modeString)
        {
            List<IrcMode> modes = new List<IrcMode>();
            string[] modeParts = modeString.Split(',');
            for (int i = 0; i < modeParts.Length; i++)
                foreach (char modeChar in modeParts[i])
                    modes.Add(new IrcMode(modeChar, (IrcModeType)i));

            return modes;
        }
    }
}
