namespace WinWeelay.Core
{
    /// <summary>
    /// Representation of a rank in an IRC channel.
    /// </summary>
    public class IrcStatusMode : IrcMode
    {
        /// <summary>
        /// Rank prefix character to be shown in front of nicknames.
        /// </summary>
        public char PrefixChar { get; private set; }

        /// <summary>
        /// Create a representation of a rank in an IRC channel.
        /// </summary>
        /// <param name="modeChar">The character that represents the mode.</param>
        /// <param name="prefixChar">Rank prefix character to be shown in front of nicknames.</param>
        public IrcStatusMode(char modeChar, char prefixChar) : base(modeChar, IrcModeType.Status)
        {
            PrefixChar = prefixChar;
        }
    }
}
