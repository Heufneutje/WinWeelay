using System.Collections.Generic;

namespace WinWeelay.Configuration
{
    /// <summary>
    /// Wrapper to display connection types in a combo box.
    /// </summary>
    public class ConnectionTypeWrapper
    {
        /// <summary>
        /// The type of relay connection.
        /// </summary>
        public RelayConnectionType ConnectionType { get; private set; }

        /// <summary>
        /// Display text for the combo box.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Create a new wrapper for a connection type.
        /// </summary>
        /// <param name="connectionType">The type of relay connection.</param>
        /// <param name="description">Display text for the combo box.</param>
        public ConnectionTypeWrapper(RelayConnectionType connectionType, string description)
        {
            ConnectionType = connectionType;
            Description = description;
        }

        /// <summary>
        /// Retrieve a user-friendly list of all connection types.
        /// </summary>
        /// <returns>A list of all connection types.</returns>
        public static IEnumerable<ConnectionTypeWrapper> GetTypes()
        {
            List<ConnectionTypeWrapper> types = new List<ConnectionTypeWrapper>();
            types.Add(new ConnectionTypeWrapper(RelayConnectionType.PlainText, "Plain connection"));
            types.Add(new ConnectionTypeWrapper(RelayConnectionType.WeechatSsl, "WeeChat SSL"));
            types.Add(new ConnectionTypeWrapper(RelayConnectionType.WebSocket, "WebSocket (Plain)"));
            types.Add(new ConnectionTypeWrapper(RelayConnectionType.WebSocketSsl, "WebSocket (SSL)"));
            return types;
        }
    }
}
