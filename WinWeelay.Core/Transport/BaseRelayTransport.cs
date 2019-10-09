using System;
using System.Threading.Tasks;
using WinWeelay.Configuration;

namespace WinWeelay.Core
{
    public abstract class BaseRelayTransport : IRelayTransport
    {
        protected RelayConfiguration _configuration;
        public bool IsConnected { get; protected set; }

        public event RelayMessageReceivedHandler RelayMessageReceived;
        public event RelayErrorHandler ErrorReceived;

        public abstract Task Connect(RelayConfiguration configuration);

        public abstract void Disconnect();

        public abstract void Write(byte[] data);

        protected void OnRelayMessageReceived(RelayMessage relayMessage)
        {
            RelayMessageReceived?.Invoke(this, new RelayMessageEventArgs(relayMessage));
        }

        protected void OnErrorReceived(Exception ex)
        {
            ErrorReceived?.Invoke(this, new RelayErrorEventArgs(ex));
        }

    }
}
