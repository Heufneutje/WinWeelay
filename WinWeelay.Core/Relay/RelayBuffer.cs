using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    /// <summary>
    /// Message buffer in WeeChat.
    /// </summary>
    public class RelayBuffer : NotifyPropertyChangedBase
    {
        private List<RelayBufferMessage> _messages;
        private bool _hasBacklog;
        private bool _hasNicklist;

        /// <summary>
        /// The connection that the buffer is created on.
        /// </summary>
        public RelayConnection Connection { get; private set; }

        /// <summary>
        /// The full internal name of the buffer.
        /// </summary>
        public string FullName { get; set; }

        private string _shortName;

        /// <summary>
        /// The short name of the buffer for display purposes.
        /// </summary>
        public string ShortName
        {
            get => _shortName;
            set
            {
                _shortName = value;
                NameChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The internal type of the buffer.
        /// </summary>
        public string BufferType { get; set; }

        /// <summary>
        /// The type of WeeChat plugin that the buffer uses.
        /// </summary>
        public string PluginType { get; set; }

        /// <summary>
        /// The buffer number for ordering.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Internal ID for the buffer.
        /// </summary>
        public string Pointer { get; set; }

        /// <summary>
        /// Currently selected nickname in the UI.
        /// </summary>
        public RelayNicklistEntry ActiveNicklistEntry { get; set; }

        /// <summary>
        /// The number of unread messages in the buffer.
        /// </summary>
        public int UnreadMessagesCount { get; set; }

        /// <summary>
        /// The number of unread highlights in the buffer.
        /// </summary>
        public int HighlightedMessagesCount { get; set; }

        /// <summary>
        /// The current maxium size of the message backlog. Starts at the configured default. Can increase when calling <c>LoadMoreMessages</c>.
        /// </summary>
        public int MaxBacklogSize { get; set; }

        /// <summary>
        /// The current messages in the buffer.
        /// </summary>
        public ReadOnlyCollection<RelayBufferMessage> Messages => _messages.AsReadOnly();

        private string _title;

        /// <summary>
        /// The buffer title/topic.
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                TitleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private string _ircChannelModes;

        /// <summary>
        /// The channel modes if this buffer is an IRC channel.
        /// </summary>
        public string IrcChannelModes
        {
            get => _ircChannelModes;
            set
            {
                _ircChannelModes = value;
                TitleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The loaded list of nicknames in the buffer.
        /// </summary>
        public ObservableCollection<RelayNicklistEntry> Nicklist { get; private set; }

        /// <summary>
        /// Child buffers for this buffer (applies to IRC server buffers for example).
        /// </summary>
        public IList<RelayBuffer> Children { get; private set; }

        /// <summary>
        /// Parent buffer for this buffer (applies to IRC channel buffers for example).
        /// </summary>
        public RelayBuffer Parent { get; set; }

        private bool _isActiveBuffer;

        /// <summary>
        /// Whether this buffer is the currently active buffer in the UI.
        /// </summary>
        public bool IsActiveBuffer
        {
            get { return _isActiveBuffer; }
            set
            {
                if (value != _isActiveBuffer)
                {
                    _isActiveBuffer = value;
                    NotifyPropertyChanged(nameof(IsActiveBuffer));
                }
            }
        }

        /// <summary>
        /// IRC server properties for the current buffer or the server buffer that this buffer is linked to.
        /// </summary>
        public IrcServer IrcServer
        {
            get
            {
                if (Parent != null && Connection.IrcServerRegistry.HasIrcServer(Parent.Pointer))
                    return Connection.IrcServerRegistry[Parent.Pointer];
                else if (Connection.IrcServerRegistry.HasIrcServer(Pointer))
                    return Connection.IrcServerRegistry[Pointer];
                else
                    return new IrcServer();
            }
        }

        #region View Model
        /// <summary>
        /// The counter badge to display in the UI.
        /// </summary>
        public string DisplayCount
        {
            get
            {
                if (HighlightedMessagesCount == 0)
                    return $"  {UnreadMessagesCount}  ";

                return $"  {HighlightedMessagesCount}  ";
            }
        }

        /// <summary>
        /// Should the badge be visible in the UI?
        /// </summary>
        public bool IsBadgeVisible => UnreadMessagesCount != 0 || HighlightedMessagesCount != 0;

        /// <summary>
        /// Whether or not message have been loaded in this buffer.
        /// </summary>
        public bool HasMessages => _messages.Any();

        /// <summary>
        /// Hex code for the background of the badge depending on the notification level.
        /// </summary>
        public string BadgeBackground
        {
            get
            {
                if (HighlightedMessagesCount != 0)
                    return "#FFFF0000";

                return "#FF2163FF";
            }
        }

        /// <summary>
        /// Event fired when this buffer's name changes.
        /// </summary>
        public event EventHandler NameChanged;

        /// <summary>
        /// Event fired when a single message is added to the buffer.
        /// </summary>
        public event MessageAddedHandler MessageAdded;

        /// <summary>
        /// Event fired when multiple messages are added to the buffer.
        /// </summary>
        public event MessageBatchAddedHandler MessageBatchAdded;

        /// <summary>
        /// Event fired when the buffer title/topic changes.
        /// </summary>
        public event EventHandler TitleChanged;

        /// <summary>
        /// Event fired when all messages in the buffer are cleared.
        /// </summary>
        public event EventHandler MessagesCleared;

        /// <summary>
        /// Event fired when the current nickname on the server changes.
        /// </summary>
        public event EventHandler CurrentNickChanged;

        /// <summary>
        /// Event fired when the user modes change.
        /// </summary>
        public event EventHandler UserModesChanged;
        #endregion

        /// <summary>
        /// Constructor for designer.
        /// </summary>
        public RelayBuffer()
        {
            _messages = new List<RelayBufferMessage>();
            Nicklist = new ObservableCollection<RelayNicklistEntry>();
            Children = new List<RelayBuffer>();
        }

        /// <summary>
        /// Create a new buffer from a received Hdata structure.
        /// </summary>
        /// <param name="connection">The connection that the buffer is created on.</param>
        /// <param name="entry">Received Hdata structure with buffer details.</param>
        public RelayBuffer(RelayConnection connection, WeechatHdataEntry entry)
        {
            UpdateBufferProperties(entry);

            Connection = connection;
            _messages = new List<RelayBufferMessage>();
            Nicklist = new ObservableCollection<RelayNicklistEntry>();
            Children = new List<RelayBuffer>();
            MaxBacklogSize = Connection.Configuration.BacklogSize;
        }

        /// <summary>
        /// Update the existing buffer with values for a received Hdata structure.
        /// </summary>
        /// <param name="entry">Received Hdata structure with buffer details.</param>
        public void UpdateBufferProperties(WeechatHdataEntry entry)
        {
            WeechatHashtable localVars = entry.GetLocalVariables();

            if (entry.DataContainsKey("name"))
                FullName = entry["name"].AsString();
            else if (entry.DataContainsKey("full_name"))
                FullName = entry["full_name"].AsString();
            else if (localVars.ContainsKey("name"))
                FullName = localVars["name"].AsString();
            else if (localVars.ContainsKey("full_name"))
                FullName = localVars["full_name"].AsString();

            ShortName = FullName;

            if (entry.DataContainsKey("short_name") && !string.IsNullOrEmpty(entry["short_name"].AsString()))
                ShortName = entry["short_name"].AsString();
            else if (localVars.ContainsKey("short_vars") && !string.IsNullOrEmpty(localVars["short_name"].AsString()))
                ShortName = localVars["short_name"].AsString();
            
            Number = entry["number"].AsInt();
            Title = entry["title"].AsString();
            Pointer = entry.GetPointer();

            if (localVars.ContainsKey("type"))
                BufferType = localVars["type"].AsString();

            if (localVars.ContainsKey("plugin"))
                PluginType = localVars["plugin"].AsString();
        }

        /// <summary>
        /// Add a single message to this buffer's message list.
        /// </summary>
        /// <param name="message">The message that should be added.</param>
        /// <param name="updateUnreadCount">Increase the number of unread messages.</param>
        public void AddSingleMessage(RelayBufferMessage message, bool updateUnreadCount)
        {
            _messages.Insert(0, message);

            if (updateUnreadCount)
            {
                UnreadMessagesCount++;

                if (message.IsHighlighted)
                    HighlightedMessagesCount++;
            }

            ShrinkMessageBuffer();
            MessageAdded?.Invoke(this, new RelayBufferMessageEventArgs(message, true, false));
        }

        /// <summary>
        /// Add multiple single message to this buffer's message list.
        /// </summary>
        /// <param name="messages">The messages that should be added.</param>
        /// <param name="isExpandedBacklog">True if the messages are received after the "Load more messages" action.</param>
        public void AddMessageBatch(IEnumerable<RelayBufferMessage> messages, bool isExpandedBacklog)
        {
            _messages.AddRange(messages);
            MessageBatchAdded?.Invoke(this, new RelayBufferMessageBatchEventsArgs(messages, false, isExpandedBacklog));
        }

        private void ShrinkMessageBuffer()
        {
            if (!Connection.Configuration.AutoShrinkBuffer)
                return;

            while (_messages.Count > MaxBacklogSize)
                _messages.RemoveAt(_messages.Count - 1);
        }

        /// <summary>
        /// Checks whether a given message has been added to this buffer.
        /// </summary>
        /// <param name="message">The message to check.</param>
        /// <returns>True if the message has been added to the buffer.</returns>
        public bool HasMessage(RelayBufferMessage message)
        {
            return _messages.Any(x => x.LinePointer == message.LinePointer);
        }

        /// <summary>
        /// Clear the received messages from this buffer. Only clears the buffer locally.
        /// </summary>
        public void ClearMessages()
        {
            _messages.Clear();
            MessagesCleared?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Update the UI when the buffer name changes.
        /// </summary>
        public void NotifyNameUpdated()
        {
            NotifyPropertyChanged(nameof(ShortName));
            NotifyPropertyChanged(nameof(FullName));
        }

        /// <summary>
        /// Update the UI when the unread count changes.
        /// </summary>
        public void NotifyMessageCountUpdated()
        {
            NotifyPropertyChanged(nameof(DisplayCount));
            NotifyPropertyChanged(nameof(IsBadgeVisible));
            NotifyPropertyChanged(nameof(BadgeBackground));
        }

        /// <summary>
        /// Select this buffer as the active buffer and load the backlog if it hasn't been loaded yet.
        /// </summary>
        public void HandleSelected()
        {
            if (Connection.ActiveBuffer != null && Connection.ActiveBuffer != this)
                Connection.ActiveBuffer.IsActiveBuffer = false;

            IsActiveBuffer = true;
            if (_hasNicklist)
                Connection.NotifyNicklistUpdated();

            if (!_hasBacklog)
            {
                LoadMoreMessages();
                _hasBacklog = true;
            }

            RequestNicklist();

            UnreadMessagesCount = 0;
            HighlightedMessagesCount = 0;

            if (Connection.IsConnected)
                Connection.OutputHandler.MarkBufferAsRead(this);

            NotifyMessageCountUpdated();
        }

        /// <summary>
        /// Deselect this buffer and set the active buffer to null.
        /// </summary>
        public void HandleUnselected()
        {
            IsActiveBuffer = false;
            Connection.NotifyNicklistUpdated();
        }

        /// <summary>
        /// Sort the nicklist by channel role, then nickname.
        /// </summary>
        public void SortNicklist()
        {
            Nicklist = new ObservableCollection<RelayNicklistEntry>(Nicklist.OrderBy(x => x.SortIndex).ThenBy(x => x.Name));
        }

        /// <summary>
        /// Sort this buffer's child buffers based on their number.
        /// </summary>
        public void SortChildren()
        {
            Children = new ObservableCollection<RelayBuffer>(Children.OrderBy(x => x.Number));
        }

        /// <summary>
        /// Get all nicknames from the message that have currently been received.
        /// </summary>
        /// <returns>A list of nicknames sorted by most recent activity.</returns>
        public IEnumerable<string> GetSortedUniqueNicks()
        {
            List<string> nicks = _messages.Where(x => !string.IsNullOrEmpty(x.Nick) && Nicklist.Any(y => y.Name == x.Nick)).OrderByDescending(x => x.Date).Select(x => x.Nick).ToList();
            nicks.AddRange(Nicklist.Select(x => x.Name));
            return nicks.Distinct();
        }

        /// <summary>
        /// Send a new message to this buffer.
        /// </summary>
        /// <param name="message">The mesage to send.</param>
        public void SendMessage(string message)
        {
            Connection.OutputHandler.Input(this, message);
        }

        /// <summary>
        /// Send a WHOIS request to the selected nickname in this buffer.
        /// </summary>
        public void SendWhois()
        {
            if (ActiveNicklistEntry != null)
                SendMessage($"/whois {ActiveNicklistEntry.Name}");
        }

        /// <summary>
        /// Kick the active nickname from this buffer (IRC channel).
        /// </summary>
        public void SendKick()
        {
            if (ActiveNicklistEntry != null)
                SendMessage($"/kick {ActiveNicklistEntry.Name}");
        }

        /// <summary>
        /// Ban the active nickname from this buffer (IRC channel).
        /// </summary>
        public void SendBan()
        {
            if (ActiveNicklistEntry != null)
                SendMessage($"/ban {ActiveNicklistEntry.Name}");
        }

        /// <summary>
        /// Kick and ban the active nickname from this buffer (IRC channel).
        /// </summary>
        public void SendKickban()
        {
            if (ActiveNicklistEntry != null)
                SendMessage($"/kickban {ActiveNicklistEntry.Name}");
        }

        /// <summary>
        /// Clear this buffer's messages both locally and on the WeeChat host.
        /// </summary>
        public void SendClear()
        {
            SendMessage("/buffer clear");
        }

        /// <summary>
        /// Reset the backlog to its default configured size.
        /// </summary>
        public void ReinitMessages()
        {
            _hasBacklog = false;
            ClearMessages();
            LoadMoreMessages();
            _hasBacklog = true;
        }

        /// <summary>
        /// Load 100 more older messages into the backlog.
        /// </summary>
        public void LoadMoreMessages()
        {
            int size;
            string messageId;

            if (_hasBacklog)
            {
                size = Messages.Count + 100;
                messageId = MessageIds.CustomGetBufferBacklogExtra;
            }
            else
            {
                size = Connection.Configuration.BacklogSize;
                messageId = MessageIds.CustomGetBufferBacklog;
            }

            MaxBacklogSize = size;

            Connection.OutputHandler.RequestBufferBacklog(this, size, messageId);
        }

        /// <summary>
        /// Clear the nicklist and request it again.
        /// </summary>
        public void ResetNicklist()
        {
            Nicklist.Clear();
            _hasNicklist = false;
            RequestNicklist();
        }

        /// <summary>
        /// Fire an event when the current nick changes.
        /// </summary>
        public void OnCurrentNickChanged()
        {
            CurrentNickChanged?.Invoke(this, EventArgs.Empty);
            foreach (RelayBuffer childBuffer in Children)
                childBuffer.OnCurrentNickChanged();
        }

        /// <summary>
        /// Fire an event when the user modes for the user on this server change.
        /// </summary>
        public void OnUserModesChanged()
        {
            UserModesChanged?.Invoke(this, EventArgs.Empty);
            foreach (RelayBuffer childBuffer in Children)
                childBuffer.OnUserModesChanged();
        }

        private void RequestNicklist()
        {
            if (!_hasNicklist)
            {
                Connection.OutputHandler.Nicklist(this, MessageIds.CustomGetNicklist);
                _hasNicklist = true;
            }
        }
    }
}
