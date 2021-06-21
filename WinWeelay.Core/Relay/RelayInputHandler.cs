using System;
using System.Collections.Generic;
using System.Linq;

namespace WinWeelay.Core
{
    /// <summary>
    /// Parser for incoming messages.
    /// </summary>
    public class RelayInputHandler
    {
        private RelayConnection _connection;
        private IRelayTransport _transport;

        /// <summary>
        /// Create a new parser for a given connection's incoming messages.
        /// </summary>
        /// <param name="connection">The connection that the messages should be handled for.</param>
        /// <param name="transport">The network stream that the messages will be received on.</param>
        public RelayInputHandler(RelayConnection connection, IRelayTransport transport)
        {
            _connection = connection;
            _transport = transport;
            _transport.RelayMessageReceived += Transport_RelayMessageReceived;
        }

        private void Transport_RelayMessageReceived(object sender, RelayMessageEventArgs args)
        {
            // If this is the first response we receive we can assume that we're now successfully logged.
            // Unfortunately when logging in fails no data is sent back so we can only assume.
            if (!_connection.IsLoggedIn && args.RelayMessage.ID != MessageIds.CustomHandshake)
                _connection.IsLoggedIn = true; 

            ParseMessage(args.RelayMessage);
        }

        private void ParseMessage(RelayMessage message)
        {
            switch (message.ID)
            {
                case MessageIds.CustomHandshake:
                    _connection.Authenticate((WeechatHashtable)message.RelayObjects.First());
                    break;
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
                case MessageIds.CustomGetColorOptions:
                    ParseColorOptions(message);
                    break;
                case MessageIds.CustomGetOptions:
                    ParseOptions(message);
                    break;
                case MessageIds.CustomGetIrcChannelProperties:
                    ParseIrcChannelProperties(message);
                    break;
                case MessageIds.CustomGetIrcServerProperties:
                    ParseIrcServerCapabilities(message);
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
                case MessageIds.BufferMoved:
                case MessageIds.BufferHidden:
                case MessageIds.BufferUnhidden:
                    ParseBuffersChanged();
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

        private void ParseColorOptions(RelayMessage message)
        {
            WeechatInfoList infolist = (WeechatInfoList)message.RelayObjects[0];
            _connection.OptionParser.ParseOptionsCached(infolist);
        }

        private void ParseOptions(RelayMessage message)
        {
            WeechatInfoList infolist = (WeechatInfoList)message.RelayObjects[0];
            _connection.OptionParser.ParseOptionsUncached(infolist);
            _connection.OnOptionsParsed();
        }

        private void ParseVersion(RelayMessage message)
        {
            WeechatInfo versionInfo = (WeechatInfo)message.RelayObjects.First();
            _connection.WeeChatVersion = versionInfo.Value;
        }

        private void ParseBufferList(RelayMessage message)
        {
            List<RelayBuffer> newBuffers = new List<RelayBuffer>();
            WeechatHdata hdata = (WeechatHdata)message.RelayObjects.First();
            for (int i = 0; i < hdata.Count; i++)
            {
                WeechatHdataEntry entry = hdata[i];
                WeechatHashtable localVars = entry.GetLocalVariables();

                RelayBuffer buffer = _connection.Buffers.FirstOrDefault(x => x.Pointer == entry.GetPointer());

                if (buffer == null && !entry["hidden"].AsBoolean())
                {
                    buffer = new RelayBuffer(_connection, entry);
                    newBuffers.Add(buffer);
                }
                else if (buffer != null && buffer.HasMessages)
                {
                    buffer.ReinitMessages();
                    buffer.ResetNicklist();
                }

                if (buffer == null)
                    continue;

                buffer.UpdateBufferProperties(entry);

                if (localVars.ContainsKey("server"))
                {
                    RelayBuffer parentBuffer = _connection.Buffers.Union(newBuffers).FirstOrDefault(x => x.BufferType == "server" && x.ShortName == localVars["server"].AsString() && x != buffer);
                    buffer.Parent = parentBuffer;

                    if (parentBuffer != null && !parentBuffer.Children.Contains(buffer))
                        parentBuffer.Children.Add(buffer);

                    if (localVars["plugin"].AsString() == "irc" && localVars["type"].AsString() == "server")
                        _connection.OutputHandler.RequestIrcServerCapabilites(buffer);
                }
            }

            foreach (RelayBuffer existingBuffer in _connection.Buffers.ToList())
            {
                WeechatHdataEntry bufferData = hdata.FirstOrDefault(x => x.GetPointer() == existingBuffer.Pointer);
                if (bufferData == null || bufferData["hidden"].AsBoolean())
                    RemoveBuffer(existingBuffer);
            }

            foreach (RelayBuffer addedBuffer in newBuffers)
            {
                _connection.Buffers.Add(addedBuffer);
                _connection.OutputHandler.RequestChannelDetails(addedBuffer);
            }

            _connection.SortBuffers();
            _connection.NotifyBuffersChanged();
        }

        private void ParseIrcServerCapabilities(RelayMessage message)
        {
            WeechatInfoList infoList = (WeechatInfoList)message.RelayObjects.First();
            Dictionary<string, WeechatRelayObject> items = infoList[0];
            _connection.IrcServerRegistry.RegisterIrcServer(items);

            RelayBuffer relayBuffer = _connection.Buffers.FirstOrDefault(x => x.Pointer == items["buffer"].AsPointer());
            if (relayBuffer != null)
                relayBuffer.OnUserModesChanged();
        }

        private void ParseIrcChannelProperties(RelayMessage message)
        {
            WeechatInfoList infoList = (WeechatInfoList)message.RelayObjects.First();
            Dictionary<string, WeechatRelayObject> items = infoList[0];

            RelayBuffer relayBuffer = _connection.Buffers.FirstOrDefault(x => x.Pointer == items["buffer"].AsPointer());
            if (relayBuffer != null)
                relayBuffer.IrcChannelModes = items["modes"].AsString();
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
            if (!hdata.Any())
                return;

            List<RelayBuffer> updatedBuffers = new List<RelayBuffer>();
            Dictionary<RelayBuffer, List<RelayBufferMessage>> newMessages = new Dictionary<RelayBuffer, List<RelayBufferMessage>>();
            int linePointerIndex = hdata.PathList.ToList().IndexOf("line_data");
            for (int i = 0; i < hdata.Count; i++)
            {
                bool isSingleLineUpdate = message.ID == MessageIds.BufferLineAdded;
                RelayBufferMessage bufferMessage = new RelayBufferMessage(hdata[i], isSingleLineUpdate, linePointerIndex);
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

                    if (hdata.Count == 1)
                        buffer.AddSingleMessage(bufferMessage, updateMessageCount);
                    else
                    {
                        if (!newMessages.ContainsKey(buffer))
                            newMessages.Add(buffer, new List<RelayBufferMessage>());
                        newMessages[buffer].Add(bufferMessage);
                    }
                        
                    if (!updatedBuffers.Contains(buffer))
                        updatedBuffers.Add(buffer);

                    if (isSingleLineUpdate && bufferMessage.IsHighlighted && !bufferMessage.IsNotified)
                        _connection.OnHighlighted(bufferMessage, buffer);

                    if (message.ID == MessageIds.BufferLineAdded)
                        HandleIrcTags(bufferMessage.Tags, buffer);
                }
            }

            foreach (KeyValuePair<RelayBuffer, List<RelayBufferMessage>> pair in newMessages)
                pair.Key.AddMessageBatch(pair.Value, message.ID == MessageIds.CustomGetBufferBacklogExtra);

            foreach (RelayBuffer updateBuffer in updatedBuffers)
                updateBuffer.NotifyMessageCountUpdated();
        }

        private void HandleIrcTags(string[] tagsArray, RelayBuffer relayBuffer)
        {
            if (tagsArray.Contains("irc_nick"))
            {
                string oldNick = GetTagValue(tagsArray, "irc_nick1");
                string newNick = GetTagValue(tagsArray, "irc_nick2");

                IrcServer ircServer = relayBuffer.IrcServer;
                if (ircServer.CurrentNick == oldNick)
                {
                    ircServer.CurrentNick = newNick;

                    RelayBuffer serverBuffer = relayBuffer;
                    if (serverBuffer.Parent != null)
                        serverBuffer = serverBuffer.Parent;

                    serverBuffer.OnCurrentNickChanged();
                }
            }
            else if (tagsArray.Contains("irc_mode"))
            {
                if (relayBuffer.BufferType == "channel")
                    _connection.OutputHandler.RequestChannelDetails(relayBuffer);
                else
                    _connection.OutputHandler.RequestIrcServerCapabilites(relayBuffer);
            }
        }

        private string GetTagValue(string[] tagsArray, string tagName)
        {
            return tagsArray.FirstOrDefault(x => x.StartsWith(tagName))?.Substring(tagName.Length + 1);
        }

        private void ParseBufferOpened(RelayMessage message)
        {
            WeechatHdata hdata = (WeechatHdata)message.RelayObjects.First();
            WeechatHdataEntry entry = hdata[0];

            string bufferName = null;
            if (entry.DataContainsKey("full_name"))
                bufferName = entry["full_name"].AsString();
            else if (entry.DataContainsKey("name"))
                bufferName = entry["name"].AsString();

            if (bufferName != null)
                _connection.OutputHandler.RequestBufferList();
        }

        private void ParseBufferClosing(RelayMessage message)
        {
            WeechatHdata hdata = (WeechatHdata)message.RelayObjects.First();
            RelayBuffer buffer = _connection.Buffers.FirstOrDefault(x => x.Pointer == hdata[0].GetPointer());
            if (buffer != null)
            {
                RemoveBuffer(buffer);
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
            WeechatHdataEntry entry = hdata[0];

            RelayBuffer buffer = _connection.Buffers.FirstOrDefault(x => x.Pointer == entry.GetPointer());

            if (buffer != null)
            {
                WeechatHashtable localVars = entry.GetLocalVariables();
                if (localVars.ContainsKey("name"))
                    buffer.FullName = localVars["name"].AsString();
                else if (localVars.ContainsKey("full_name"))
                    buffer.FullName = localVars["full_name"].AsString();

                buffer.ShortName = buffer.FullName;
                if (entry.DataContainsKey("short_name") && !string.IsNullOrEmpty(entry["short_name"].AsString()))
                    buffer.ShortName = entry["short_name"].AsString();

                buffer.NotifyNameUpdated();
            }
        }

        private void ParseBuffersChanged()
        {
            _connection.OutputHandler.RequestBufferList();
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

        private void RemoveBuffer(RelayBuffer buffer)
        {
            _connection.IsRefreshingBuffers = true;
            _connection.Buffers.Remove(buffer);
            foreach (RelayBuffer rootBuffer in _connection.RootBuffers)
                if (rootBuffer.Children.Contains(buffer))
                    rootBuffer.Children.Remove(buffer);

            _connection.CloseBuffer(buffer);
            _connection.IsRefreshingBuffers = false;
        }
    }
}
