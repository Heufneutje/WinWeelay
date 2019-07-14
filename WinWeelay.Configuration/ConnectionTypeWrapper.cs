using System.Collections.Generic;

namespace WinWeelay.Configuration
{
    public class ConnectionTypeWrapper
    {
        public RelayConnectionType ConnectionType { get; private set; }
        public string Description { get; private set; }

        public ConnectionTypeWrapper(RelayConnectionType connectionType, string description)
        {
            ConnectionType = connectionType;
            Description = description;
        }

        public static IEnumerable<ConnectionTypeWrapper> GetTypes()
        {
            List<ConnectionTypeWrapper> types = new List<ConnectionTypeWrapper>();
            types.Add(new ConnectionTypeWrapper(RelayConnectionType.PlainText, "Plain Connection"));
            types.Add(new ConnectionTypeWrapper(RelayConnectionType.WeechatSsl, "WeeChat SSL"));
            return types;
        }
    }
}
