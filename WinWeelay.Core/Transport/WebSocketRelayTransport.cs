using System;
using System.ComponentModel;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;
using WinWeelay.Configuration;

namespace WinWeelay.Core
{
    public class WebSocketRelayTransport : BaseRelayTransport
    {
        private WebsocketClient _webSocket;
        private SynchronizationContext _synchronizationContext;
        private bool _useSsl;
        
        public WebSocketRelayTransport(bool useSsl)
        {
            _useSsl = useSsl;
            _synchronizationContext = SynchronizationContext.Current;
        }

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
                RelayMessage relayMessage = new RelayMessage(message.Binary);
                _synchronizationContext.Post(delegate { OnRelayMessageReceived(relayMessage); }, null);
            }
        }

        public override void Disconnect()
        {
            IsConnected = false;
            _webSocket.Dispose();
        }

        public override void Write(byte[] data)
        {
            if (IsConnected)
                _webSocket.Send(data);
        }
    }
}
