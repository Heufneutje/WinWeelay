using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;
using WinWeelay.Configuration;

namespace WinWeelay.Core
{
    /// <summary>
    /// Plain text or secure WebSocket connection.
    /// </summary>
    public class WebSocketRelayTransport : BaseRelayTransport
    {
        private WebsocketClient _webSocket;
        private readonly bool _useSsl;

        /// <summary>
        /// Create a new WebSocket transport instance.
        /// </summary>
        /// <param name="useSsl">Connect to a secure WebSocket.</param>
        public WebSocketRelayTransport(bool useSsl)
        {
            _useSsl = useSsl;
        }

        /// <summary>
        /// Connect to a WeeChat instance with the given configuration.
        /// </summary>
        /// <param name="configuration">Main configuration.</param>
        /// <returns>Async task.</returns>
        public override async Task Connect(RelayConfiguration configuration)
        {
            _configuration = configuration;
            string protocol = _useSsl ? "wss" : "ws";

            try
            {
                _webSocket = new WebsocketClient(new Uri($"{protocol}://{_configuration.Hostname}:{_configuration.Port}/{_configuration.WebSocketPath}"));
                _webSocket.MessageReceived.Subscribe(msg => HandleMessage(msg));

                await Task.WhenAny(_webSocket.Start(), Task.Delay(5000));
                if (!_webSocket.IsRunning)
                    throw new IOException("Connection timed out.");

                IsConnected = true;
            }
            catch (Exception ex)
            {
                _webSocket?.Dispose();
                OnErrorReceived(ex);
            }
        }

        private void HandleMessage(ResponseMessage message)
        {
            if (message.MessageType == WebSocketMessageType.Binary)
            {
                RelayMessage relayMessage = new(message.Binary);
                OnRelayMessageReceived(relayMessage);
            }
        }

        /// <summary>
        /// Disconnect from the WeeChat instance.
        /// </summary>
        public override void Disconnect()
        {
            IsConnected = false;
            _webSocket?.Dispose();
        }

        /// <summary>
        /// Write raw data to the relay connection.
        /// </summary>
        /// <param name="data">The data array to write.</param>
        public override void Write(byte[] data)
        {
            if (IsConnected)
                _webSocket.Send(data);
        }
    }
}
