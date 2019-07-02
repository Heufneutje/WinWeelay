namespace WinWeelay.Core
{
    /// <summary>
    /// Type of relay objects.
    /// </summary>
    public enum WeechatType
    {
        /// <summary>
        /// Character data type.
        /// </summary>
        CHR,

        /// <summary>
        /// 32-bit integer data type.
        /// </summary>
        INT,

        /// <summary>
        /// 64-bit integer data type.
        /// </summary>
        LON,

        /// <summary>
        /// String data type.
        /// </summary>
        STR,

        /// <summary>
        /// Buffer data type.
        /// </summary>
        BUF,

        /// <summary>
        /// Pointer data type.
        /// </summary>
        PTR,

        /// <summary>
        /// Time data type.
        /// </summary>
        TIM,

        /// <summary>
        /// Hashtable data table.
        /// </summary>
        HTB,

        /// <summary>
        /// Hdata data type.
        /// </summary>
        HDA,

        /// <summary>
        /// Info data type.
        /// </summary>
        INF,

        /// <summary>
        /// Info list data type.
        /// </summary>
        INL,

        /// <summary>
        /// Array data type.
        /// </summary>
        ARR,

        /// <summary>
        /// Unknown data type.
        /// </summary>
        UNKNOWN
    }
}
