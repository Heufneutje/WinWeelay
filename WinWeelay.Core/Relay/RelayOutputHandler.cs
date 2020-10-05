using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinWeelay.Core
{
    /// <summary>
    /// Handler for sending messages.
    /// </summary>
    public class RelayOutputHandler
    {
        private RelayConnection _connection;
        private IRelayTransport _transport;
        private bool _useBatch;
        private List<byte> _messageBatch;

        /// <summary>
        /// Create a new handler for a given connection's outgoing messages.
        /// </summary>
        /// <param name="connection">The connection that the handler applies to.</param>
        /// <param name="transport">The network stream that the messages will be sent to.</param>
        public RelayOutputHandler(RelayConnection connection, IRelayTransport transport)
        {
            _connection = connection;
            _transport = transport;
            _messageBatch = new List<byte>();
        }

        /// <summary>
        /// Buffer and hold all outgoing messages until <c>EndMessageBatch</c> is called.
        /// </summary>
        public void BeginMessageBatch()
        {
            if (_useBatch)
                throw new InvalidOperationException("The output handler is already handling a batch.");

            _useBatch = true;
        }

        /// <summary>
        /// Send all buffered messages received since the last <c>BeginMessageBatch</c> call.
        /// </summary>
        public void EndMessageBatch()
        {
            if (!_useBatch)
                throw new InvalidOperationException("The output handler not currently handling a batch.");

            _useBatch = false;
            SendMessage(_messageBatch.ToArray());
            _messageBatch.Clear();
        }

        /// <summary>
        /// Send a raw message to the WeeChat host.
        /// </summary>
        /// <param name="message">The content of the message.</param>
        /// <param name="id">Optional. The ID of the message.</param>
        public void SendMessage(string message, string id = null)
        {
            if (!string.IsNullOrEmpty(id))
                message = $"({id}) {message}";

            List<byte> msgBytes = new UTF8Encoding().GetBytes(message).ToList();
            msgBytes.Add(0x0A);

            if (_useBatch)
                _messageBatch.AddRange(msgBytes);
            else
                SendMessage(msgBytes.ToArray());
        }

        private void SendMessage(byte[] msgBytes)
        {
            try
            {
                _transport.Write(msgBytes);
            }
            catch (Exception ex)
            {
                _connection.HandleException(ex);
            }
        }

        /// <summary>
        /// Retrieve the list of buffers.
        /// </summary>
        public void RequestBufferList()
        {
            Hdata($"buffer:gui_buffers(*)", "number,name,full_name,short_name,title,hidden,local_variables", MessageIds.CustomGetBufferList);
        }

        /// <summary>
        /// Retrieve the message unread counter badge data.
        /// </summary>
        public void RequestHotlist()
        {
            Infolist("hotlist", id: MessageIds.CustomGetHotlist);
        }

        /// <summary>
        /// Retrieve all color options.
        /// </summary>
        public void RequestColorOptions()
        {
            RequestOptions("weechat.color.*", MessageIds.CustomGetColorOptions);
        }

        /// <summary>
        /// Retrieve all options that match a given filter.
        /// </summary>
        /// <param name="filter">The filter to apply to the options.</param>
        /// <param name="id">Optional. The ID of the message.</param>
        public void RequestOptions(string filter, string id = MessageIds.CustomGetOptions)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                Infolist("option", id: id);
            }
            else
            {
                if (!filter.Contains("*"))
                    filter = $"*{filter}*";

                Infolist("option", $"/s {filter}", id);
            }
        }

        /// <summary>
        /// Change the value of an option in WeeChat.
        /// </summary>
        /// <param name="option">The name of the option that will be changed.</param>
        /// <param name="value">The new value of the option.</param>
        public void SetOption(string option, string value)
        {
            Input(_connection.GetCoreBuffer(), $"/set {option} {value}");
        }

        /// <summary>
        /// Reset the unread counter of a given buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public void MarkBufferAsRead(RelayBuffer buffer)
        {
            if (_connection.Configuration.SyncReadMessages)
            {
                BeginMessageBatch();
                Input(buffer, "/buffer set hotlist -1");
                Input(buffer, "/input set_unread_current_buffer");
                EndMessageBatch();
            }
        }

        /// <summary>
        /// Retrieve the backlog for a given buffer.
        /// </summary>
        /// <param name="buffer">The buffer to retrieve the backlog for.</param>
        /// <param name="backlogSize">The number of message to retrieve.</param>
        /// <param name="messageId">The ID for the request.</param>
        public void RequestBufferBacklog(RelayBuffer buffer, int backlogSize, string messageId)
        {
            Hdata($"buffer:{buffer.Pointer}/own_lines/last_line(-{backlogSize})/data", "buffer,date,prefix,message,highlight,tags_array", messageId);
        }

        /// <summary>
        /// Authenticate with the WeeChat host.
        /// </summary>
        /// <param name="password">The relay password.</param>
        /// <param name="useCompression">Whether to use compression for the message data. Should be true under normal circumstances.</param>
        public void Init(string password, bool useCompression)
        {
            if (useCompression)
                SendMessage($"init password={password}");
            else
                SendMessage($"init password={password},compression=off");
        }

        /// <summary>
        /// Retrieve a raw Hdata structure.
        /// </summary>
        /// <param name="path">The path to the data in WeeChat.</param>
        /// <param name="keys">The fields to retrieve in the data.</param>
        /// <param name="id">Optional. The ID of the request.</param>
        public void Hdata(string path, string keys = null, string id = null)
        {
            if (string.IsNullOrEmpty(keys))
                SendMessage($"hdata {path}", id);
            else
                SendMessage($"hdata {path} {keys}", id);
        }

        /// <summary>
        /// Retrieve a raw Info structure.
        /// </summary>
        /// <param name="name">The name of the data to retrieve.</param>
        /// <param name="id">Optional. The ID of the request.</param>
        public void Info(string name, string id = null)
        {
            SendMessage($"info {name}", id);
        }

        /// <summary>
        /// Retrieve a raw Infolist structure.
        /// </summary>
        /// <param name="name">The name of the list to retrieve.</param>
        /// <param name="arguments">The arguments for retrieving the list.</param>
        /// <param name="id">Optional. The ID of the request.</param>
        public void Infolist(string name, string arguments = null, string id = null)
        {
            if (string.IsNullOrEmpty(arguments))
                SendMessage($"infolist {name}", id);
            else if (!string.IsNullOrEmpty(arguments))
                SendMessage($"infolist {name} {arguments}", id);
        }

        /// <summary>
        /// Retrieve the nickname list for a given buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="id">Optional. The ID of the request.</param>
        public void Nicklist(RelayBuffer buffer = null, string id = null)
        {
            if (buffer == null)
                SendMessage("nicklist", id);
            else
                SendMessage($"nicklist {buffer.Pointer}", id);
        }

        /// <summary>
        /// Send a new message to a given buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="message">The contents of the message.</param>
        public void Input(RelayBuffer buffer, string message)
        {
            SendMessage($"input {buffer.Pointer} {message}");
        }

        /// <summary>
        /// Start receiving messages automatically.
        /// </summary>
        /// <param name="buffers">The buffers to retrieve message for.</param>
        /// <param name="signalType">The signal type(s) to receive.</param>
        /// <seealso cref="WeechatSignalType"/>
        public void Sync(string buffers = null, WeechatSignalType? signalType = null)
        {
            SendSyncDesync("sync", buffers, signalType);
        }

        /// <summary>
        /// Stop receiving messages automatically.
        /// </summary>
        /// <param name="buffer">Optional. The buffer to retrieve message for.</param>
        /// <param name="signalType">The signal type(s) to stop receiving.</param>
        /// <seealso cref="WeechatSignalType"/>
        public void Desync(string buffer = null, WeechatSignalType? signalType = null)
        {
            SendSyncDesync("desync", buffer, signalType);
        }

        /// <summary>
        /// Send a ping request.
        /// </summary>
        public void Ping()
        {
            SendMessage($"ping {DateTime.UtcNow.Ticks}");
        }

        /// <summary>
        /// Cleanly close the connection to the WeeChat host.
        /// </summary>
        public void Quit()
        {
            SendMessage("quit");
        }

        private void SendSyncDesync(string command, string buffer = null, WeechatSignalType? signalType = null)
        {
            if (string.IsNullOrEmpty(buffer))
                SendMessage(command);
            else if (signalType != null)
            {
                string signals = GetSignals(signalType.Value);
                if (!string.IsNullOrEmpty(signals))
                    SendMessage($"{command} {buffer} {signals}");
                else
                    SendMessage($"{command} {buffer}");
            }
            else
                SendMessage($"{command} {buffer}");
        }

        private string GetSignals(WeechatSignalType signalType)
        {
            List<string> signals = new List<string>();
            if ((signalType & WeechatSignalType.Buffer) != WeechatSignalType.None)
                signals.Add("buffer");
            if ((signalType & WeechatSignalType.Buffers) != WeechatSignalType.None)
                signals.Add("buffers");
            if ((signalType & WeechatSignalType.Upgrade) != WeechatSignalType.None)
                signals.Add("upgrade");
            if ((signalType & WeechatSignalType.Nicklist) != WeechatSignalType.None)
                signals.Add("nicklist");

            if (signals.Any())
                return string.Join(",", signals);

            return null;
        }
    }
}
