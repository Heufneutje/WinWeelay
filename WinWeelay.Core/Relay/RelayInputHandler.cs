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
                case MessageIds.CustomGetBufferList:
                    ParseBufferList(message);
                    break;
                case MessageIds.BufferLineAdded:
                case MessageIds.CustomGetBufferBacklog:
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
        
        private void ParseBufferLines(RelayMessage message)
        {
            WeechatHdata hdata = (WeechatHdata)message.RelayObjects.First();
            List<RelayBuffer> updatedBuffers = new List<RelayBuffer>();
            for (int i = 0; i < hdata.Count; i++)
            {
                RelayBufferMessage bufferMessage = new RelayBufferMessage(hdata[i]);
                RelayBuffer buffer = _connection.Buffers.FirstOrDefault(x => x.Pointer == bufferMessage.BufferPointer);
                if (buffer != null)
                {
                    buffer.AddMessage(bufferMessage);
                    if (!updatedBuffers.Contains(buffer))
                        updatedBuffers.Add(buffer);
                }
            }

            foreach (RelayBuffer updateBuffer in updatedBuffers)
                updateBuffer.NotifyMessagesUpdated();
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
