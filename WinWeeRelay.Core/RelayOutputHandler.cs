using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace WinWeeRelay.Core
{
    public class RelayOutputHandler
    {
        private NetworkStream _networkStream;

        public RelayOutputHandler(NetworkStream networkStream)
        {
            _networkStream = networkStream;
        }

        public void SendMessage(string message, string id = null)
        {
            if (!string.IsNullOrEmpty(id))
                message = $"({id}) {message}";

            List<byte> msgBytes = new UTF8Encoding().GetBytes(message).ToList();
            msgBytes.Add(0x0A);
            byte[] msgByteArray = msgBytes.ToArray();
            _networkStream.Write(msgByteArray, 0, msgByteArray.Length);
        }

        public void RequestBufferList()
        {
            Hdata("buffer:gui_buffers(*)", "number,name,title", MessageIds.CustomGetBufferList);
        }

        public void RequestBufferBacklog(RelayBuffer buffer, int backlogSize)
        {
            Hdata($"buffer:{buffer.Pointer}/lines/last_line(-{backlogSize})/data", id: MessageIds.CustomGetBufferBacklog);
            buffer.HasBacklog = true;
        }

        public void Init(string password)
        {
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
            if (string.IsNullOrEmpty(pointer))
                SendMessage($"infolist {name}", id);
            else if (string.IsNullOrEmpty(arguments))
                SendMessage($"infolist {name} {pointer}", id);
            else
                SendMessage($"infolist {name} {pointer} {arguments}", id);
        }

        public void Nicklist(string buffer = null, string id = null)
        {
            if (string.IsNullOrEmpty(buffer))
                SendMessage("nicklist", id);
            else
                SendMessage($"nicklist {buffer}", id);
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
