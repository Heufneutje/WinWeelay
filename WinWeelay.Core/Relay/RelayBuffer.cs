using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    public class RelayBuffer : NotifyPropertyChangedBase
    {
        private List<RelayBufferMessage> _messages;
        private bool _hasBacklog;
        private bool _hasNicklist;
        private int _backlogOffset;

        public RelayConnection Connection { get; private set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NameChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int Number { get; set; }
        public string Pointer { get; set; }
        public RelayNicklistEntry ActiveNicklistEntry { get; set; }
        public int UnreadMessagesCount { get; set; }
        public int HighlightedMessagesCount { get; set; }
        public ReadOnlyCollection<RelayBufferMessage> Messages
        {
            get
            {
                return _messages.AsReadOnly();
            }
        }

        private string _title;
        

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                TitleChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public ObservableCollection<RelayNicklistEntry> Nicklist { get; private set; }

        #region View Model
        public IEnumerable<RelayBufferMessage> MessagesToHighlight
        {
            get
            {
                return _messages.Where(x => x.IsHighlighted && !x.IsNotified);
            }
        }

        public string DisplayCount
        {
            get
            {
                if (HighlightedMessagesCount == 0)
                    return $"  {UnreadMessagesCount}  ";

                return $"  {HighlightedMessagesCount}  ";
            }
        }

        public bool IsBadgeVisible
        {
            get
            {
                return UnreadMessagesCount != 0 || HighlightedMessagesCount != 0;
            }
        }

        public string BadgeBackground
        {
            get
            {
                if (HighlightedMessagesCount != 0)
                    return "#FFFF0000";

                return "#FF2163FF";
            }
        }

        public event EventHandler NameChanged;
        public event MessageAddedHandler MessageAdded;
        public event EventHandler TitleChanged;
        public event EventHandler MessagesCleared;
        #endregion

        public RelayBuffer()
        {
            _messages = new List<RelayBufferMessage>();
            Nicklist = new ObservableCollection<RelayNicklistEntry>();
        }

        public RelayBuffer(RelayConnection connection, WeechatHdataEntry entry)
        {
            UpdateBufferProperties(entry);

            Connection = connection;
            _messages = new List<RelayBufferMessage>();
            Nicklist = new ObservableCollection<RelayNicklistEntry>();
        }

        public void UpdateBufferProperties(WeechatHdataEntry entry)
        {
            if (entry.DataContainsKey("name"))
                Name = entry["name"].AsString();
            else
            {
                WeechatHashtable table = (WeechatHashtable)entry["local_variables"];
                Name = table["name"].AsString();
            }

            Number = entry["number"].AsInt();
            Title = entry["title"].AsString();
            Pointer = entry.GetPointer();
        }

        public void AddSingleMessage(RelayBufferMessage message, bool updateCount)
        {
            _messages.Insert(0, message);

            if (updateCount)
            {
                UnreadMessagesCount++;

                if (message.IsHighlighted)
                    HighlightedMessagesCount++;
            }

            MessageAdded?.Invoke(this, new RelayBufferMessageEventArgs(message, true, false));
        }

        public void AddMessageBatch(IEnumerable<RelayBufferMessage> messages, bool isExpandedBacklog)
        {
            _messages.AddRange(messages);

            foreach (RelayBufferMessage message in messages)
                MessageAdded?.Invoke(this, new RelayBufferMessageEventArgs(message, false, isExpandedBacklog));
        }

        public bool HasMessage(RelayBufferMessage message)
        {
            return _messages.Contains(message);
        }

        public void ClearMessages()
        {
            _messages.Clear();
            MessagesCleared?.Invoke(this, EventArgs.Empty);
        }

        public void NotifyNameUpdated()
        {
            NotifyPropertyChanged(nameof(Name));
        }

        public void NotifyMessageCountUpdated()
        {
            NotifyPropertyChanged(nameof(DisplayCount));
            NotifyPropertyChanged(nameof(IsBadgeVisible));
            NotifyPropertyChanged(nameof(BadgeBackground));
        }

        public void HandleSelected()
        {
            Connection.ActiveBuffer = this;
            if (_hasNicklist)
                Connection.NotifyNicklistUpdated();

            if (!_hasBacklog)
            {
                LoadMoreMessages();
                _hasBacklog = true;
            }

            if (!_hasNicklist)
            {
                Connection.OutputHandler.Nicklist(this, MessageIds.CustomGetNicklist);
                _hasNicklist = true;
            }

            UnreadMessagesCount = 0;
            HighlightedMessagesCount = 0;

            if (Connection.IsConnected)
                Connection.OutputHandler.MarkBufferAsRead(this);

            NotifyMessageCountUpdated();
        }

        public void HandleUnselected()
        {
            Connection.ActiveBuffer = null;
            Connection.NotifyNicklistUpdated();
        }

        public void SortNicklist()
        {
            Nicklist = new ObservableCollection<RelayNicklistEntry>(Nicklist.OrderBy(x => x.SortIndex).ThenBy(x => x.Name));
        }

        public IEnumerable<string> GetSortedUniqueNicks()
        {
            List<string> nicks = _messages.Where(x => !string.IsNullOrEmpty(x.Nick)).OrderByDescending(x => x.Date).Select(x => x.Nick).ToList();
            nicks.AddRange(Nicklist.Select(x => x.Name));
            return nicks.Distinct();
        }

        public void SendMessage(string message)
        {
            Connection.OutputHandler.Input(this, message);
        }

        public void SendWhois()
        {
            if (ActiveNicklistEntry != null)
                SendMessage($"/whois {ActiveNicklistEntry.Name}");
        }

        public void SendKick()
        {
            if (ActiveNicklistEntry != null)
                SendMessage($"/kick {ActiveNicklistEntry.Name}");
        }

        public void SendBan()
        {
            if (ActiveNicklistEntry != null)
                SendMessage($"/ban {ActiveNicklistEntry.Name}");
        }

        public void SendKickban()
        {
            if (ActiveNicklistEntry != null)
                SendMessage($"/kickban {ActiveNicklistEntry.Name}");
        }

        public void LoadMoreMessages()
        {
            int size = Connection.Configuration.BacklogSize;
            string messageId = MessageIds.CustomGetBufferBacklog;

            if (_hasBacklog)
            {
                _backlogOffset += 100;
                size += _backlogOffset;
                messageId = MessageIds.CustomGetBufferBacklogExtra;
            }

            Connection.OutputHandler.RequestBufferBacklog(this, size, messageId);
        }
    }
}
