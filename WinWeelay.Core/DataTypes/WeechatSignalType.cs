namespace WinWeelay.Core
{
    /// <summary>
    /// Type for receiving different signal types for syncing.
    /// </summary>
    public enum WeechatSignalType
    {
        /// <summary>
        /// Subscribe to no events.
        /// </summary>
        None = 0,

        /// <summary>
        /// Subscribe to buffer metadata changes.
        /// </summary>
        Buffers = 1,

        /// <summary>
        /// Subscribe to WeeChat application upgrades.
        /// </summary>
        Upgrade = 2,

        /// <summary>
        /// Subscribe to buffer messages.
        /// </summary>
        Buffer = 4,

        /// <summary>
        /// Subscribe to nicklist updates.
        /// </summary>
        Nicklist = 8
    }
}
