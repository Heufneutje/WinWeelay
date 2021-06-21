namespace WinWeelay.Core
{
    /// <summary>
    /// Types of IRC modes.
    /// </summary>
    public enum IrcModeType
    {
        /// <summary>
        /// A mode that manages a list.
        /// </summary>
        List = 0,

        /// <summary>
        /// A mode that requires a parameter to be set and a parameter to unset.
        /// </summary>
        ParamSetUnset = 1,

        /// <summary>
        /// A mode that requires a parameter to be set, but no parameter to unset.
        /// </summary>
        ParamSet = 2,

        /// <summary>
        /// A mode that requires no parameter to be set or unset.
        /// </summary>
        NoParam = 3,

        /// <summary>
        /// A mode that represents a rank in an IRC channel.
        /// </summary>
        Status = 4
    }
}
