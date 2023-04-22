namespace WinWeelay.Core
{
    /// <summary>
    /// Generic relay object, base class.
    /// </summary>
    public abstract class WeechatRelayObject
    {
        /// <summary>
        /// Type of the relay object.
        /// </summary>
        public WeechatType Type { get; set; } = WeechatType.Unknown;
    }
}
