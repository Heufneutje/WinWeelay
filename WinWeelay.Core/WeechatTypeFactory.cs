namespace WinWeelay.Core
{
    /// <summary>
    /// Helper class to get the enum value that matches the 3-character type string from the relay data.
    /// </summary>
    public static class WeechatTypeFactory
    {
        /// <summary>
        /// Get the enum value that matches the 3-character type string from the relay data.
        /// </summary>
        /// <param name="relayType">3-character type string.</param>
        /// <returns>The enum value that matches the relay data.</returns>
        public static WeechatType GetWeechatType(string relayType)
        {
            return relayType.ToUpper() switch
            {
                "CHR" => WeechatType.Char,
                "INT" => WeechatType.Int32,
                "LON" => WeechatType.Int64,
                "STR" => WeechatType.String,
                "BUF" => WeechatType.Buffer,
                "PTR" => WeechatType.Pointer,
                "TIM" => WeechatType.Time,
                "HTB" => WeechatType.Hashtable,
                "HDA" => WeechatType.Hdata,
                "INF" => WeechatType.Info,
                "INL" => WeechatType.Infolist,
                "ARR" => WeechatType.Array,
                _ => WeechatType.Unknown,
            };
        }
    }
}
