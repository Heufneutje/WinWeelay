using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    public class RelayInputHandler
    {
        private RelayConnection _connection;
        private Stream _networkStream;
        private BackgroundWorker _inputWorker;

        public RelayInputHandler(RelayConnection connection, Stream networkStream)
        {
            _connection = connection;
            _networkStream = networkStream;

            _inputWorker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            _inputWorker.DoWork += InputWorker_DoWork;
            _inputWorker.ProgressChanged += InputWorker_ProgressChanged;
            _inputWorker.RunWorkerAsync();
        }

        private void InputWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!(e.UserState is byte[]))
                return;

            RelayMessage relayMessage = new RelayMessage((byte[])e.UserState);
            ParseMessage(relayMessage);
        }

        private void InputWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BufferedStream reader = null;
            try
            {
                reader = new BufferedStream(_networkStream);
            }
            catch (Exception ex)
            {
                if (_connection.IsConnected)
                    _connection.HandleException(ex);
                return;
            }

            while (true)
            {
                try
                {
                    List<byte> bytes = new List<byte>();

                    byte[] buffer = new byte[4];
                    int read = reader.Read(buffer, 0, buffer.Length);

                    if (read == 4)
                    {
                        byte[] lengthBytes = ArrayHelper.CopyOfRange(buffer, 0, 4);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(lengthBytes);

                        int length = BitConverter.ToInt32(lengthBytes, 0);
                        bytes.AddRange(buffer);

                        length -= 4;
                        while (length > 0)
                        {
                            bytes.Add((byte)reader.ReadByte());
                            length -= 1;
                        }

                        _inputWorker.ReportProgress(0, bytes.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    if (_connection.IsConnected)
                        _connection.HandleException(ex);
                    break;
                }

                if (_inputWorker.CancellationPending)
                    break;
            }
        }

        private void ParseMessage(RelayMessage message)
        {
            switch (message.ID)
            {
                case MessageIds.CustomGetVersion:
                    ParseVersion(message);
                    break;
                case MessageIds.CustomGetBufferList:
                    ParseBufferList(message);
                    break;
                case MessageIds.CustomGetHotlist:
                    ParseHotlist(message);
                    break;
                case MessageIds.BufferLineAdded:
                case MessageIds.CustomGetBufferBacklog:
                case MessageIds.CustomGetBufferBacklogExtra:
                    ParseBufferLines(message);
                    break;
                case MessageIds.BufferOpened:
                    ParseBufferOpened(message);
                    break;
                case MessageIds.BufferClosing:
                    ParseBufferClosing(message);
                    break;
                case MessageIds.BufferTitleChanged:
                    ParseBufferTitleChanged(message);
                    break;
                case MessageIds.BufferRenamed:
                    ParseBufferRenamed(message);
                    break;
                case MessageIds.BufferCleared:
                    ParseBufferCleared(message);
                    break;
                case MessageIds.Nicklist:
                case MessageIds.NicklistDiff:
                case MessageIds.CustomGetNicklist:
                    ParseNicklist(message);
                    break;
                case MessageIds.Upgrade:
                    ParseUpgrade();
                    break;
                case MessageIds.UpgradeEnded:
                    ParseUpgradeEnded();
                    break;
            }
        }

        private void ParseVersion(RelayMessage message)
        {
            WeechatInfo versionInfo = (WeechatInfo)message.RelayObjects.First();
            _connection.WeeChatVersion = versionInfo.Value;
        }

        private void ParseBufferList(RelayMessage message)
        {
            List<RelayBuffer> buffers = new List<RelayBuffer>();
            WeechatHdata hdata = (WeechatHdata)message.RelayObjects.First();
            for (int i = 0; i < hdata.Count; i++)
            {
                WeechatHdataEntry entry = hdata[i];
                RelayBuffer buffer = new RelayBuffer(_connection, entry);
                buffers.Add(buffer);
            }
            _connection.Buffers.Clear();

            foreach (RelayBuffer addedBuffer in buffers)
                _connection.Buffers.Add(addedBuffer);
            _connection.SortBuffers();
            _connection.NotifyBuffersChanged();
        }

        private void ParseHotlist(RelayMessage message)
        {
            WeechatInfoList list = (WeechatInfoList)message.RelayObjects.First();
            foreach (Dictionary<string, WeechatRelayObject> listItem in list)
            {
                RelayBuffer buffer = _connection.Buffers.FirstOrDefault(x => x.Pointer == listItem["buffer_pointer"].AsPointer());
                if (buffer == null)
                    continue;

                buffer.HighlightedMessagesCount = 0;
                buffer.UnreadMessagesCount = 0;

                int priority = listItem["priority"].AsInt();
                switch (priority)
                {
                    case 0: // Join/part messages. Don't need to display a buffer update for that.
                        break;
                    case 1: // Normal message.
                        buffer.UnreadMessagesCount += listItem["count_01"].AsInt();
                        break;
                    case 2: // Private message.
                        buffer.HighlightedMessagesCount += listItem["count_02"].AsInt();
                        break;
                    case 3: // Highlight.
                        buffer.HighlightedMessagesCount = listItem["count_03"].AsInt();
                        goto case 2;
                }

                buffer.NotifyMessageCountUpdated();
            }
        }

        private void ParseBufferLines(RelayMessage message)
        {
            WeechatHdata hdata = (WeechatHdata)message.RelayObjects.First();
            List<RelayBuffer> updatedBuffers = new List<RelayBuffer>();
            for (int i = 0; i < hdata.Count; i++)
            {
                bool isSingleLineUpdate = message.ID == MessageIds.BufferLineAdded;
                RelayBufferMessage bufferMessage = new RelayBufferMessage(hdata[i], isSingleLineUpdate);
                RelayBuffer buffer = _connection.Buffers.FirstOrDefault(x => x.Pointer == bufferMessage.BufferPointer);
                if (buffer != null && !buffer.HasMessage(bufferMessage))
                {
                    bool updateMessageCount = isSingleLineUpdate;

                    if (updateMessageCount && !bufferMessage.Tags.Contains("irc_privmsg") && !bufferMessage.Tags.Contains("irc_notice"))
                        updateMessageCount = false;

                    if (_connection.ActiveBuffer == buffer)
                    {
                        updateMessageCount = false;
                        _connection.OutputHandler.MarkBufferAsRead(buffer);
                    }

                    buffer.AddMessage(bufferMessage, updateMessageCount, message.ID == MessageIds.CustomGetBufferBacklogExtra);
                    if (!updatedBuffers.Contains(buffer))
                        updatedBuffers.Add(buffer);
                }
            }

            foreach (RelayBuffer updateBuffer in updatedBuffers)
                updateBuffer.NotifyMessageCountUpdated();
        }

        private void ParseBufferOpened(RelayMessage message)
        {
            WeechatHdata hdata = (WeechatHdata)message.RelayObjects.First();
            RelayBuffer buffer = new RelayBuffer(_connection, hdata[0]);
            _connection.Buffers.Add(buffer);
            _connection.SortBuffers();
            _connection.NotifyBuffersChanged();
        }

        private void ParseBufferClosing(RelayMessage message)
        {
            WeechatHdata hdata = (WeechatHdata)message.RelayObjects.First();
            RelayBuffer buffer = _connection.Buffers.FirstOrDefault(x => x.Pointer == hdata[0].GetPointer());
            if (buffer != null)
            {
                _connection.Buffers.Remove(buffer);
                _connection.NotifyBufferClosed(buffer);
                _connection.NotifyBuffersChanged();
            }
        }

        private void ParseBufferTitleChanged(RelayMessage message)
        {
            WeechatHdata hdata = (WeechatHdata)message.RelayObjects.First();
            WeechatHdataEntry entry = hdata[0];
            RelayBuffer buffer = _connection.Buffers.FirstOrDefault(x => x.Pointer == entry.GetPointer());
            if (buffer != null)
            {
                buffer.Title = entry["title"].AsString();
                _connection.NotifyNicklistUpdated();
            }
        }

        private void ParseBufferCleared(RelayMessage message)
        {
            WeechatHdata hdata = (WeechatHdata)message.RelayObjects.First();
            RelayBuffer buffer = _connection.Buffers.FirstOrDefault(x => x.Pointer == hdata[0].GetPointer());
            if (buffer != null)
            {
                buffer.ClearMessages();
                buffer.NotifyMessageCountUpdated();
            }
        }

        private void ParseBufferRenamed(RelayMessage message)
        {
            WeechatHdata hdata = (WeechatHdata)message.RelayObjects.First();
            RelayBuffer buffer = _connection.Buffers.FirstOrDefault(x => x.Pointer == hdata[0].GetPointer());

            if (buffer != null)
            {
                WeechatHashtable table = (WeechatHashtable)hdata[0]["local_variables"];
                buffer.Name = table["name"].ToString();
                buffer.NotifyNameUpdated();
            }
        }

        private void ParseNicklist(RelayMessage message)
        {
            WeechatHdata hdata = (WeechatHdata)message.RelayObjects.First();

            bool nicklistCleared = message.ID != MessageIds.Nicklist;
            List<RelayBuffer> updatedBuffers = new List<RelayBuffer>();
            for (int i = 0; i < hdata.Count; i++)
            {
                RelayBuffer buffer = _connection.Buffers.FirstOrDefault(x => x.Pointer == hdata[i].GetPointer(0));
                if (buffer == null)
                    continue;

                RelayNicklistEntry nicklistEntry = new RelayNicklistEntry(hdata[i], buffer);
                if (nicklistEntry.IsGroup)
                    continue;

                if (!nicklistCleared)
                {
                    buffer.Nicklist.Clear();
                    nicklistCleared = true;
                }

                char diffChar = '+';
                if (hdata.KeyList.Contains("_diff"))
                    diffChar = hdata[i]["_diff"].AsChar();

                switch (diffChar)
                {
                    case '+':
                        if (!buffer.Nicklist.Contains(nicklistEntry))
                            buffer.Nicklist.Add(nicklistEntry);
                        break;
                    case '-':
                        RelayNicklistEntry entryToRemove = buffer.Nicklist.FirstOrDefault(x => x.Name == nicklistEntry.Name);
                        if (entryToRemove != null)
                            buffer.Nicklist.Remove(entryToRemove);
                        break;
                    case '*':
                        RelayNicklistEntry entryToUpdate = buffer.Nicklist.FirstOrDefault(x => x.Name == nicklistEntry.Name);
                        if (entryToUpdate != null)
                            entryToUpdate.Update(nicklistEntry);
                        break;
                }

                if (!updatedBuffers.Contains(buffer))
                    updatedBuffers.Add(buffer);
            }
            foreach (RelayBuffer updateBuffer in updatedBuffers)
                updateBuffer.SortNicklist();
            _connection.NotifyNicklistUpdated();
        }

        private void ParseUpgrade()
        {
            _connection.OutputHandler.Desync();
            _connection.Buffers.Clear();
            _connection.NotifyBuffersChanged();
        }

        private void ParseUpgradeEnded()
        {
            _connection.OutputHandler.Sync();
            _connection.OutputHandler.RequestBufferList();
        }

        public void CancelInputWorker()
        {
            _inputWorker.CancelAsync();
        }
    }
}
