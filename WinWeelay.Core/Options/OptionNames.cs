namespace WinWeelay.Core
{
    /// <summary>
    /// Names of all color options.
    /// </summary>
    public static class OptionNames
    {
        /// <summary>
        /// Color for window separators (when split) and separators beside bars (like nicklist).
        /// </summary>
        public const string WeechatColorSeparator = "weechat.color.separator";

        /// <summary>
        /// Text color for chat.
        /// </summary>
        public const string WeechatColorChat = "weechat.color.chat";

        /// <summary>
        /// Text color for time in chat window.
        /// </summary>
        public const string WeechatColorChatTime = "weechat.color.chat_time";

        /// <summary>
        /// Text color for time delimiters.
        /// </summary>
        public const string WeechatColorChatTImeDelimiters = "weechat.color.chat_time_delimiters";

        /// <summary>
        /// Text color for error prefix.
        /// </summary>
        public const string WeechatColorChatPrefixError = "weechat.color.chat_prefix_error";

        /// <summary>
        /// Text color for network prefix.
        /// </summary>
        public const string WeechatColorChatPrefixNetwork = "weechat.color.chat_prefix_network";

        /// <summary>
        /// Text color for action prefix.
        /// </summary>
        public const string WeechatColorChatPrefixAction = "weechat.color.chat_prefix_action";

        /// <summary>
        /// Text color for join prefix.
        /// </summary>
        public const string WeechatColorChatPrefixJoin = "weechat.color.chat_prefix_join";

        /// <summary>
        /// Text color for quit prefix.
        /// </summary>
        public const string WeechatColorChatPrefixQuit = "weechat.color.chat_prefix_quit";

        /// <summary>
        /// Text color for "+" when prefix is too long.
        /// </summary>
        public const string WeechatColorChatPrefixMore = "weechat.color.chat_prefix_more";

        /// <summary>
        /// Text color for suffix (after prefix).
        /// </summary>
        public const string WeechatColorChatPrefixSuffix = "weechat.color.chat_prefix_suffix";

        /// <summary>
        /// Text color for buffer names.
        /// </summary>
        public const string WeechatColorChatBuffer = "weechat.color.chat_buffer";

        /// <summary>
        /// Text color for server names.
        /// </summary>
        public const string WeechatColorChatServer = "weechat.color.chat_server";

        /// <summary>
        /// Text color for channel names.
        /// </summary>
        public const string WeechatColorChatChannel = "weechat.color.chat_channel";

        /// <summary>
        /// Text color for nicks in chat window: used in some server messages and as fallback when a nick color is not found; most of times nick color comes from option weechat.color.chat_nick_colors.
        /// </summary>
        public const string WeechatColorChatNick = "weechat.color.chat_nick";

        /// <summary>
        /// Text color for local nick in chat window.
        /// </summary>
        public const string WeechatColorChatNickSelf = "weechat.color.chat_nick_self";

        /// <summary>
        /// Text color for other nick in private buffer.
        /// </summary>
        public const string WeechatColorChatNickOther = "weechat.color.chat_nick_other";

        /// <summary>
        /// Text color for hostnames.
        /// </summary>
        public const string WeechatColorChatHost = "weechat.color.chat_host";

        /// <summary>
        /// Text color for delimiters.
        /// </summary>
        public const string WeechatColorChatDelimiters = "weechat.color.chat_delimiters";

        /// <summary>
        /// Text color for highlighted prefix.
        /// </summary>
        public const string WeechatColorChatHighlight = "weechat.color.chat_highlight";

        /// <summary>
        /// Text color for unread data marker.
        /// </summary>
        public const string WeechatColorChatReadMarker = "weechat.color.chat_read_marker";

        /// <summary>
        /// Text color for marker on lines where text sought is found.
        /// </summary>
        public const string WeechatColorChatTextFound = "weechat.color.chat_text_found";

        /// <summary>
        /// Text color for values.
        /// </summary>
        public const string WeechatColorChatValue = "weechat.color.chat_value";

        /// <summary>
        /// Text color for buffer name (before prefix, when many buffers are merged with same number).
        /// </summary>
        public const string WeechatColorChatPrefixBuffer = "weechat.color.chat_prefix_buffer";

        /// <summary>
        /// Text color for tags after messages (displayed with command /debug tags).
        /// </summary>
        public const string WeechatColorChatTags = "weechat.color.chat_tags";

        /// <summary>
        /// Text color for chat when window is inactive (not current selected window).
        /// </summary>
        public const string WeechatColorChatInactiveWindow = "weechat.color.chat_inactive_window";

        /// <summary>
        /// Text color for chat when line is inactive (buffer is merged with other buffers and is not selected).
        /// </summary>
        public const string WeechatColorChatInactiveBuffer = "weechat.color.chat_inactive_buffer";

        /// <summary>
        /// Text color for inactive buffer name (before prefix, when many buffers are merged with same number and if buffer is not selected).
        /// </summary>
        public const string WeechatColorChatPrefixBufferInactiveBuffer = "weechat.color.chat_prefix_buffer_inactive_buffer";

        /// <summary>
        /// Text color for offline nick (not in nicklist any more); this color is used only if option weechat.look.color_nick_offline is enabled.
        /// </summary>
        public const string WeechatColorChatNickOffline = "weechat.color.chat_nick_offline";

        /// <summary>
        /// Text color for offline nick with highlight; this color is used only if option weechat.look.color_nick_offline is enabled.
        /// </summary>
        public const string WeechatColorChatNickOfflineHighlight = "weechat.color.chat_nick_offline_highlight";

        /// <summary>
        /// Background color for offline nick with highlight; this color is used only if option weechat.look.color_nick_offline is enabled.
        /// </summary>
        public const string WeechatColorChatNickPrefix = "weechat.color.chat_nick_prefix";

        /// <summary>
        /// Color for nick suffix (string displayed after nick in prefix).
        /// </summary>
        public const string WeechatColorChatNickSuffix = "weechat.color.chat_nick_suffix";

        /// <summary>
        /// Text color for emphasized text (for example when searching text); this option is used only if option weechat.look.emphasized_attributes is an empty string (default value).
        /// </summary>
        public const string WeechatColorEmphasized = "weechat.color.emphasized";

        /// <summary>
        /// Text color for message displayed when the day has changed.
        /// </summary>
        public const string WeechatColorChatDayChange = "weechat.color.chat_day_change";

        /// <summary>
        /// Text color for null values (undefined).
        /// </summary>
        public const string WeechatColorChatValueNull = "weechat.color.chat_value_null";
    }
}
