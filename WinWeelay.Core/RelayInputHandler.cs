using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    public class RelayInputHandler
    {
        private RelayConnection _connection;
        private NetworkStream _networkStream;
        private BackgroundWorker _inputWorker;

        public RelayInputHandler(RelayConnection connection, NetworkStream networkStream)
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
            Debug.WriteLine(relayMessage);
        }

        private void InputWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BufferedStream reader = new BufferedStream(_networkStream);
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
                catch (IOException)
                {
                    // TODO: Handle this properly.
                    break;
                }

                if (_inputWorker.CancellationPending)
                    break;
            }
        }

        public void ParseMessage(RelayMessage message)
        {
            WeechatHdata hdata;
            RelayBuffer buffer;
            List<RelayBuffer> updatedBuffers;

            switch (message.ID)
            {
                case MessageIds.CustomGetBufferList:
                    List<RelayBuffer> buffers = new List<RelayBuffer>();
                    hdata = (WeechatHdata)message.RelayObjects.First();
                    for (int i = 0; i < hdata.Count; i++)
                    {
                        WeechatHdataEntry entry = hdata[i];
                        buffer = new RelayBuffer(_connection, entry);
                        buffers.Add(buffer);
                    }
                    _connection.Buffers.Clear();
                    _connection.Buffers.AddRange(buffers);
                    _connection.NotifyBuffersChanged();
                    break;
                case MessageIds.BufferLineAdded:
                case MessageIds.CustomGetBufferBacklog:
                    hdata = (WeechatHdata)message.RelayObjects.First();

                    updatedBuffers = new List<RelayBuffer>();
                    for (int i = 0; i < hdata.Count; i++)
                    {
                        RelayBufferMessage bufferMessage = new RelayBufferMessage(hdata[i]);

                        buffer = _connection.Buffers.FirstOrDefault(x => x.Pointer == bufferMessage.BufferPointer);
                        if (buffer != null)
                        {
                            buffer.AddMessage(bufferMessage);
                            if (!updatedBuffers.Contains(buffer))
                                updatedBuffers.Add(buffer);
                        }
                    }

                    foreach (RelayBuffer updateBuffer in updatedBuffers)
                        updateBuffer.NotifyMessagesUpdated();
                    break;
                case MessageIds.Nicklist:
                case MessageIds.NicklistDiff:
                case MessageIds.CustomGetNicklist:
                    hdata = (WeechatHdata)message.RelayObjects.First();

                    bool nicklistCleared = message.ID != MessageIds.Nicklist;
                    updatedBuffers = new List<RelayBuffer>();
                    for (int i = 0; i < hdata.Count; i++)
                    {
                        RelayNicklistEntry nicklistEntry = new RelayNicklistEntry(hdata[i]);
                        if (nicklistEntry.IsGroup)
                            continue;

                        buffer = _connection.Buffers.FirstOrDefault(x => x.Pointer == hdata[i].GetPointer(0));
                        if (buffer != null)
                        {
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
                    }
                    foreach (RelayBuffer updateBuffer in updatedBuffers)
                        updateBuffer.SortNicklist();
                    _connection.NotifyNicklistUpdated();
                    break;
                case MessageIds.Upgrade:
                    _connection.OutputHandler.Desync();
                    _connection.Buffers.Clear();
                    _connection.NotifyBuffersChanged();
                    break;
                case MessageIds.UpgradeEnded:
                    _connection.OutputHandler.Sync();
                    _connection.OutputHandler.RequestBufferList();
                    break;
            }
        }
    }
}
