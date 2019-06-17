using System;
using System.Collections.Generic;
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
        public string Title { get; set; }
        public List<RelayNicklistEntry> Nicklist { get; private set; }

        public string MessageBuffer
        {
            get
            {
                return string.Join(Environment.NewLine, _messages.OrderBy(x => x.Date));
            }
        }

        public RelayBuffer()
        {
            _messages = new List<RelayBufferMessage>();
            Nicklist = new List<RelayNicklistEntry>();
        }

        public RelayBuffer(RelayConnection connection, WeechatHdataEntry entry)
        {
            Name = entry["name"].AsString();
            Number = entry["number"].AsInt();
            Title = entry["title"].AsString();
            Pointer = entry.GetPointer();

            _connection = connection;
            _messages = new List<RelayBufferMessage>();
            Nicklist = new List<RelayNicklistEntry>();
        }

        public override string ToString()
        {
            return Name;
        }

        public void AddMessage(RelayBufferMessage message)
        {
            _messages.Add(message);
        }

        public void NotifyMessagesUpdated()
        {
            NotifyPropertyChanged(nameof(MessageBuffer));
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
        }

        public void SortNicklist()
        {
            Nicklist = Nicklist.OrderBy(x => x.SortIndex).ThenBy(x => x.Name).ToList();
        }
    }
}
