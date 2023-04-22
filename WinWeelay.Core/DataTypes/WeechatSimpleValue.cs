namespace WinWeelay.Core.DataTypes
{
    /// <summary>
    /// Container for basic value types.
    /// </summary>
    /// <typeparam name="T">Type of the basic value.</typeparam>
    public class WeechatSimpleValue<T> : WeechatRelayObject where T : notnull
    {
        /// <summary>
        /// The basic value.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Create a new basic value container from a value and matching relay object type
        /// </summary>
        /// <param name="value">The basic value.</param>
        /// <param name="type">Type of the relay object.</param>
        public WeechatSimpleValue(T value, WeechatType type)
        {
            Value = value;
            Type = type;
        }
    }
}
