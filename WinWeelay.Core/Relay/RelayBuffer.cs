using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WinWeelay.Core
{
    public class RelayBuffer : NotifyPropertyChangedBase
    {
        private List<RelayBufferMessage> _messages;
        private bool _hasBacklog;
        private bool _hasNicklist;

        public RelayConnection Connection { get; private set; }
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

        public event MessageAddedHandler MessageAdded;

        #region View Model
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

            Connection = connection;
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

            MessageAdded?.Invoke(this, new RelayBufferMessageEventArgs(message, message.Date >= _messages.Max(x => x.Date)));
        }

        public bool HasMessage(RelayBufferMessage message)
        {
            return _messages.Contains(message);
        }

        public void ClearMessages()
        {
            _messages.Clear();
        }

        public void NotifyMessagesUpdated()
        {
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
            Connection.ActiveBuffer = this;
            if (_hasNicklist)
                Connection.NotifyNicklistUpdated();

            if (!_hasBacklog)
            {
                Connection.OutputHandler.RequestBufferBacklog(this, Connection.Configuration.BacklogSize);
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
    }
}
