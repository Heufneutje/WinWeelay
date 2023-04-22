using System;
using System.Threading;
using System.Threading.Tasks;
using WinWeelay.Configuration;

namespace WinWeelay.Core
{
    /// <summary>
    /// Base class for connections.
    /// </summary>
    public abstract class BaseRelayTransport : IRelayTransport
    {
        private readonly SynchronizationContext _synchronizationContext;

        /// <summary>
        /// Main configuration.
        /// </summary>
        protected RelayConfiguration _configuration;

        /// <summary>
        /// Is a connection currently established?
        /// </summary>
        public bool IsConnected { get; protected set; }

        /// <summary>
        /// Event fired when a raw messages is received.
        /// </summary>
        public event RelayMessageReceivedHandler RelayMessageReceived;

        /// <summary>
        /// Event fired when an error occurs.
        /// </summary>
        public event RelayErrorHandler ErrorReceived;

        /// <summary>
        /// Base constructor.
        /// </summary>
        protected BaseRelayTransport()
        {
            _synchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Connect to a WeeChat instance with the given configuration.
        /// </summary>
        /// <param name="configuration">Main configuration.</param>
        /// <returns>Async task.</returns>
        public abstract Task Connect(RelayConfiguration configuration);

        /// <summary>
        /// Disconnect from the WeeChat instance.
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// Write raw data to the relay connection.
        /// </summary>
        /// <param name="data">The data array to write.</param>
        public abstract void Write(byte[] data);

        /// <summary>
        /// Fire event for a received raw message.
        /// </summary>
        /// <param name="relayMessage">The received message.</param>
        protected void OnRelayMessageReceived(RelayMessage relayMessage)
        {
            _synchronizationContext.Post(delegate
            {
                RelayMessageReceived?.Invoke(this, new RelayMessageEventArgs(relayMessage));
            }, null);
        }

        /// <summary>
        /// Fire event for an error that has occurred.
        /// </summary>
        /// <param name="ex">The error that has occurred.</param>
        protected void OnErrorReceived(Exception ex)
        {
            ErrorReceived?.Invoke(this, new RelayErrorEventArgs(ex));
        }
    }
}
