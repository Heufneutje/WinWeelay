namespace WinWeelay.Core
{
    /// <summary>
    /// Default and custom Custom identifiers for relay messages.
    /// </summary>
    public static class MessageIds
    {
        /// <summary>
        /// Custom identifier used to retrieve the list of buffers.
        /// </summary>
        public const string CustomGetBufferList = "getbuflist";

        /// <summary>
        /// Custom identifier used to retrieve the backlog of a buffer.
        /// </summary>
        public const string CustomGetBufferBacklog = "getbufbacklog";

        /// <summary>
        /// Custom identifier used to load more message from the backlog of a buffer.
        /// </summary>
        public const string CustomGetBufferBacklogExtra = "getbufbacklogextra";

        /// <summary>
        /// Custom identifier used to retrieve the nicknames that are in a channel buffer.
        /// </summary>
        public const string CustomGetNicklist = "getnicklist";

        /// <summary>
        /// Custom identifier used to retrieve the hotlist numbers for all buffers.
        /// </summary>
        public const string CustomGetHotlist = "gethotlist";

        /// <summary>
        /// Custom identifier used to retrieve WeeChat's version.
        /// </summary>
        public const string CustomGetVersion = "getversion";

        /// <summary>
        /// Custom identifier used to retrieve WeeChat's color options.
        /// </summary>
        public const string CustomGetColorOptions = "getcoloroptions";

        /// <summary>
        /// Custom identifier used to retrieve WeeChat's options.
        /// </summary>
        public const string CustomGetOptions = "getoptions";

        /// <summary>
        /// Custom identifier used to retrieve IRC server capabilites.
        /// </summary>
        public const string CustomGetIrcServerProperties = "getircserverprops";

        /// <summary>
        /// Custom identifier used to retrieve IRC channel details.
        /// </summary>
        public const string CustomGetIrcChannelProperties = "getircchanprops";

        /// <summary>
        /// Custom identifier for performing a handshake.
        /// </summary>
        public const string CustomHandshake = "handshake";

        /// <summary>
        /// Default identifier used when a new buffer is opened.
        /// </summary>
        public const string BufferOpened = "_buffer_opened";

        /// <summary>
        /// Default identifier used the title of a buffer changes,
        /// </summary>
        public const string BufferTitleChanged = "_buffer_title_changed";

        /// <summary>
        /// Default identifier used when a new buffer's messages are cleared.
        /// </summary>
        public const string BufferCleared = "_buffer_cleared";

        /// <summary>
        /// Default identifier used when a line is added to a buffer.
        /// </summary>
        public const string BufferLineAdded = "_buffer_line_added";

        /// <summary>
        /// Default identifier used when a buffer closes.
        /// </summary>
        public const string BufferClosing = "_buffer_closing";

        /// <summary>
        /// Default identifier used when a buffer's name changes.
        /// </summary>
        public const string BufferRenamed = "_buffer_renamed";

        /// <summary>
        /// Default identifier used when a buffer's order is changed.
        /// </summary>
        public const string BufferMoved = "_buffer_moved";

        /// <summary>
        /// Default identifier used when a buffer is hidden.
        /// </summary>
        public const string BufferHidden = "_buffer_hidden";

        /// <summary>
        /// Default identifier used when a hidden buffer is shown.
        /// </summary>
        public const string BufferUnhidden = "_buffer_unhidden";

        /// <summary>
        /// Default identifier used when the nicklist is retrieved.
        /// </summary>
        public const string Nicklist = "_nicklist";

        /// <summary>
        /// Default identifier used when the nicklist changes.
        /// </summary>
        public const string NicklistDiff = "_nicklist_diff";

        /// <summary>
        /// Default identifier used before WeeChat's is upgraded to a new version.
        /// </summary>
        public const string Upgrade = "_upgrade";

        /// <summary>
        /// Default identifier used after WeeChat's is upgraded to a new version.
        /// </summary>
        public const string UpgradeEnded = "_upgrade_ended";
    }
}
