using System.Threading.Tasks;
using WinWeelay.Configuration;

namespace WinWeelay.Core
{
    /// <summary>
    /// Interface for connections.
    /// </summary>
    public interface IRelayTransport
    {
        /// <summary>
        /// Event fired when a raw messages is received.
        /// </summary>
        event RelayMessageReceivedHandler RelayMessageReceived;

        /// <summary>
        /// Event fired when an error occurs.
        /// </summary>
        event RelayErrorHandler ErrorReceived;

        /// <summary>
        /// Is a connection currently established?
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connect to a WeeChat instance with the given configuration.
        /// </summary>
        /// <param name="configuration">Main configuration.</param>
        /// <returns>Async task.</returns>
        Task Connect(RelayConfiguration configuration);

        /// <summary>
        /// Disconnect from the WeeChat instance.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Write raw data to the relay connection.
        /// </summary>
        /// <param name="data">The data array to write.</param>
        void Write(byte[] data);
    }
}
