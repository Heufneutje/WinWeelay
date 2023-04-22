using System.ComponentModel;

namespace WinWeelay.Core
{
    /// <summary>
    /// Type of relay objects.
    /// </summary>
    public enum WeechatType
    {
        /// <summary>
        /// Unknown data type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Character data type.
        /// </summary>
        Char = 1,

        /// <summary>
        /// 32-bit integer data type.
        /// </summary>
        Int32 = 2,

        /// <summary>
        /// 64-bit integer data type.
        /// </summary>
        Int64 = 3,

        /// <summary>
        /// String data type.
        /// </summary>
        String = 4,

        /// <summary>
        /// Buffer data type.
        /// </summary>
        Buffer = 5,

        /// <summary>
        /// Pointer data type.
        /// </summary>
        Pointer = 6,

        /// <summary>
        /// Time data type.
        /// </summary>
        Time = 7,

        /// <summary>
        /// Hashtable data table.
        /// </summary>
        Hashtable = 8,

        /// <summary>
        /// Hdata data type.
        /// </summary>
        Hdata = 9,

        /// <summary>
        /// Info data type.
        /// </summary>
        Info = 10,

        /// <summary>
        /// Info list data type.
        /// </summary>
        Infolist = 11,

        /// <summary>
        /// Array data type.
        /// </summary>
        Array = 12
    }
}
