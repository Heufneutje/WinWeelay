using System;

namespace WinWeelay.Core
{
    /// <summary>
    /// Info representation of a relay object.
    /// </summary>
    public class WeechatInfo : WeechatRelayObject
    {
        /// <summary>
        /// The value of the pair.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// The key name of the pair.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Create a new info object.
        /// </summary>
        /// <param name="name">The key name of the pair.</param>
        /// <param name="value">The value of the pair.</param>
        public WeechatInfo(string name, string value)
        {
            Name = name;
            Value = value;
            Type = WeechatType.INF;
        }
    }
}
