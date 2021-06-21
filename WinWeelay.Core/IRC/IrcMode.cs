namespace WinWeelay.Core
{
    /// <summary>
    /// Representation of a mode for an IRC user or channel.
    /// </summary>
    public class IrcMode
    {
        /// <summary>
        /// The character that represents the mode.
        /// </summary>
        public char ModeChar { get; private set; }

        /// <summary>
        /// The type of mode (for setting and unsetting).
        /// </summary>
        public IrcModeType ModeType { get; private set; }

        /// <summary>
        /// Create a representation of a mode for an IRC user or channel.
        /// </summary>
        /// <param name="modeChar">The letter that represents the mode.</param>
        /// <param name="modeType">The type of mode (for setting and unsetting).</param>
        public IrcMode(char modeChar, IrcModeType modeType)
        {
            ModeChar = modeChar;
            ModeType = modeType;
        }
    }
}
