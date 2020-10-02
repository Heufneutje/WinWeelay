﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinWeelay.Core
{
    public class RelayOutputHandler
    {
        private RelayConnection _connection;
        private IRelayTransport _transport;
        private bool _useBatch;
        private List<byte> _messageBatch;

        public RelayOutputHandler(RelayConnection connection, IRelayTransport transport)
        {
            _connection = connection;
            _transport = transport;
            _messageBatch = new List<byte>();
        }

        public void BeginMessageBatch()
        {
            if (_useBatch)
                throw new InvalidOperationException("The output handler is already handling a batch.");

            _useBatch = true;
        }

        public void EndMessageBatch()
        {
            if (!_useBatch)
                throw new InvalidOperationException("The output handler not currently handling a batch.");

            _useBatch = false;
            SendMessage(_messageBatch.ToArray());
            _messageBatch.Clear();
        }

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

        public void RequestBufferList()
        {
            Hdata($"buffer:gui_buffers(*)", "number,name,full_name,short_name,title,hidden,local_variables", MessageIds.CustomGetBufferList);
        }

        public void RequestHotlist()
        {
            Infolist("hotlist", id: MessageIds.CustomGetHotlist);
        }

        public void RequestColorOptions()
        {
            RequestOptions("weechat.color.*", MessageIds.CustomGetColorOptions);
        }

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

                Infolist("option", string.Empty, $"/s {filter}", id);
            }
        }

        public void SetOption(string option, string value)
        {
            Input(_connection.GetCoreBuffer(), $"/set {option} {value}");
        }

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

        public void RequestBufferBacklog(RelayBuffer buffer, int backlogSize, string messageId)
        {
            Hdata($"buffer:{buffer.Pointer}/own_lines/last_line(-{backlogSize})/data", "buffer,date,prefix,message,highlight,tags_array", messageId);
        }

        public void Init(string password, bool useCompression)
        {
            if (useCompression)
                SendMessage($"init password={password}");
            else
                SendMessage($"init password={password},compression=off");
        }

        public void Hdata(string path, string keys = null, string id = null)
        {
            if (string.IsNullOrEmpty(keys))
                SendMessage($"hdata {path}", id);
            else
                SendMessage($"hdata {path} {keys}", id);
        }

        public void Info(string name, string id = null)
        {
            SendMessage($"info {name}", id);
        }

        public void Infolist(string name, string pointer = null, string arguments = null, string id = null)
        {
            if (string.IsNullOrEmpty(pointer) && string.IsNullOrEmpty(arguments))
                SendMessage($"infolist {name}", id);
            else if (string.IsNullOrEmpty(arguments))
                SendMessage($"infolist {name} {pointer}", id);
            else
                SendMessage($"infolist {name} {arguments}", id);
        }

        public void Nicklist(RelayBuffer buffer = null, string id = null)
        {
            if (buffer == null)
                SendMessage("nicklist", id);
            else
                SendMessage($"nicklist {buffer.Pointer}", id);
        }

        public void Input(RelayBuffer buffer, string message)
        {
            SendMessage($"input {buffer.Pointer} {message}");
        }

        public void Sync(string buffers = null, WeechatSignalType? signalType = null)
        {
            SendSyncDesync("sync", buffers, signalType);
        }

        public void Desync(string buffer = null, WeechatSignalType? signalType = null)
        {
            SendSyncDesync("desync", buffer, signalType);
        }

        public void Ping()
        {
            SendMessage($"ping {DateTime.UtcNow.Ticks}");
        }

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
