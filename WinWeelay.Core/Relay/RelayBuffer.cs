using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WinWeelay.Core
{
    public class RelayBuffer : NotifyPropertyChangedBase
    {
        private List<RelayBufferMessage> _messages;
        private RelayConnection _connection;
        private bool _hasBacklog;
        private bool _hasNicklist;

        public string Name { get; set; }
        public int Number { get; set; }
        public string Pointer { get; set; }
        public int UnreadMessagesCount { get; set; }
        public int HighlightedMessagesCount { get; set; }

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
                NotifyPropertyChanged(nameof(Title));
            }
        }
        public ObservableCollection<RelayNicklistEntry> Nicklist { get; private set; }

        #region View Model
        public string MessageBuffer
        {
            get
            {
                return string.Join(Environment.NewLine, _messages.OrderBy(x => x.Date));
            }
        }

        public IEnumerable<RelayBufferMessage> MessagesToHighlight
        {
            get
            {
                return _messages.Where(x => x.IsHighlighted && !x.IsNotified);
            }
        }

        public int DisplayCount
        {
            get
            {
                if (HighlightedMessagesCount == 0)
                    return UnreadMessagesCount;

                return HighlightedMessagesCount;
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

                return "#FF42CEF5";
            }
        }
        #endregion

        public RelayBuffer()
        {
            _messages = new List<RelayBufferMessage>();
            Nicklist = new ObservableCollection<RelayNicklistEntry>();
        }

        public RelayBuffer(RelayConnection connection, WeechatHdataEntry entry)
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

            _connection = connection;
            _messages = new List<RelayBufferMessage>();
            Nicklist = new ObservableCollection<RelayNicklistEntry>();
        }

        public override string ToString()
        {
            return Name + UnreadMessagesCount;
        }

        public void AddMessage(RelayBufferMessage message, bool updateCount)
        {
            _messages.Add(message);

            if (updateCount)
            {
                UnreadMessagesCount++;

                if (message.IsHighlighted)
                    HighlightedMessagesCount++;
            }
        }

        public void ClearMessages()
        {
            _messages.Clear();
        }

        public void NotifyMessagesUpdated()
        {
            NotifyPropertyChanged(nameof(MessageBuffer));
            NotifyMessageCountUpdated();
        }

        public void NotifyMessageCountUpdated()
        {
            NotifyPropertyChanged(nameof(DisplayCount));
            NotifyPropertyChanged(nameof(IsBadgeVisible));
            NotifyPropertyChanged(nameof(BadgeBackground));
        }

        public void HandleSelected()
        {
            _connection.ActiveBuffer = this;
            if (_hasNicklist)
                _connection.NotifyNicklistUpdated();

            if (!_hasBacklog)
            {
                _connection.OutputHandler.RequestBufferBacklog(this, _connection.Configuration.BacklogSize);
                _hasBacklog = true;
            }

            if (!_hasNicklist)
            {
                _connection.OutputHandler.Nicklist(this, MessageIds.CustomGetNicklist);
                _hasNicklist = true;
            }

            UnreadMessagesCount = 0;
            HighlightedMessagesCount = 0;

            if (_connection.IsConnected)
                _connection.OutputHandler.MarkBufferAsRead(this);

            NotifyMessageCountUpdated();
        }

        public void HandleUnselected()
        {
            _connection.ActiveBuffer = null;
            _connection.NotifyNicklistUpdated();
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
    }
}
