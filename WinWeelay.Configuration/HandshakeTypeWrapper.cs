using System.Collections.Generic;

namespace WinWeelay.Configuration
{
    /// <summary>
    /// Wrapper to display handshake types in a combo box.
    /// </summary>
    public class HandshakeTypeWrapper
    {
        /// <summary>
        /// The type of handshake.
        /// </summary>
        public HandshakeType HandshakeType { get; private set; }

        /// <summary>
        /// Display text for the combo box.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Create a new wrapper for a handshake.
        /// </summary>
        /// <param name="handshakeType">The type of handshake.</param>
        /// <param name="description">Display text for the combo box.</param>
        public HandshakeTypeWrapper(HandshakeType handshakeType, string description)
        {
            HandshakeType = handshakeType;
            Description = description;
        }

        /// <summary>
        /// Retrieve a user-friendly list of all handshake types.
        /// </summary>
        /// <returns>A list of all handshake types.</returns>
        public static IEnumerable<HandshakeTypeWrapper> GetTypes()
        {
            List<HandshakeTypeWrapper> types = new();
            types.Add(new HandshakeTypeWrapper(HandshakeType.Legacy, "Legacy"));
            types.Add(new HandshakeTypeWrapper(HandshakeType.Modern, "Modern"));
            return types;
        }
    }
}
