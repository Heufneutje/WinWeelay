using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WinWeelay.Core
{
    public class RelayBuffer : NotifyPropertyChangedBase
    {
        private List<RelayBufferMessage> _messages;

        public string Name { get; set; }
        public int Number { get; set; }
        public string Pointer { get; set; }
        public string Title { get; set; }
        public bool HasBacklog { get; set; }

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
        }

        public RelayBuffer(WeechatHdataEntry entry)
        {
            Name = entry["name"].AsString();
            Number = entry["number"].AsInt();
            Title = entry["title"].AsString();
            Pointer = entry.GetPointer();

            _messages = new List<RelayBufferMessage>();
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
    }
}
