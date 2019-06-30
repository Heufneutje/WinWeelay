using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    public class RelayBufferMessage : IComparable
    {
        public string BufferPointer { get; private set; }
        public DateTime Date { get; private set; }
        public DateTime DatePrinted { get; private set; }
        public bool IsDisplayed { get; private set; }
        public bool IsHighlighted { get; private set; }
        public string[] Tags { get; private set; }
        public string Prefix { get; private set; }
        public string Message { get; private set; }
        public string Nick { get; private set; }
        public string MessageType { get; private set; }
        public bool IsNotified { get; set; }

        private string _unformattedPrefix;
        public string UnformattedPrefix
        {
            get
            {
                if (_unformattedPrefix == null)
                    _unformattedPrefix = FormattingHelper.StripWeechatFormatting(Prefix);
                return _unformattedPrefix;
            }
        }

        private string _unformattedMessage;
        public string UnformattedMessage
        {
            get
            {
                if (_unformattedMessage == null)
                    _unformattedMessage = FormattingHelper.StripWeechatFormatting(Message);
                return _unformattedMessage;
            }
        }

        public RelayBufferMessage(WeechatHdataEntry entry, bool showNotifications)
        {
            BufferPointer = entry["buffer"].AsPointer();
            Date = entry["date"].AsTime();
            DatePrinted = entry["date_printed"].AsTime();
            IsDisplayed = Convert.ToBoolean(entry["displayed"].AsBoolean());
            IsHighlighted = Convert.ToBoolean(entry["highlight"].AsBoolean());
            Tags = entry["tags_array"].AsArray().ToStringArray();
            Prefix = entry["prefix"].AsString();
            Message = entry["message"].AsString();

            string[] tags = entry["tags_array"].AsArray().ToStringArray();
            string nickTag = tags.FirstOrDefault(x => x.StartsWith("nick_"));
            if (nickTag != null)
                Nick = nickTag.Substring(5);

            string messageTypeTag = tags.LastOrDefault(x => x.StartsWith("irc_"));
            if (messageTypeTag != null)
                MessageType = messageTypeTag.Substring(4);

            if (!showNotifications)
                IsNotified = true;
        }

        public override bool Equals(object obj)
        {
            return obj is RelayBufferMessage message &&
                   BufferPointer == message.BufferPointer &&
                   Date == message.Date &&
                   DatePrinted == message.DatePrinted &&
                   IsDisplayed == message.IsDisplayed &&
                   IsHighlighted == message.IsHighlighted &&
                   Message == message.Message &&
                   Nick == message.Nick &&
                   MessageType == message.MessageType;
        }

        public override int GetHashCode()
        {
            var hashCode = 1396436438;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BufferPointer);
            hashCode = hashCode * -1521134295 + Date.GetHashCode();
            hashCode = hashCode * -1521134295 + DatePrinted.GetHashCode();
            hashCode = hashCode * -1521134295 + IsDisplayed.GetHashCode();
            hashCode = hashCode * -1521134295 + IsHighlighted.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nick);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MessageType);
            return hashCode;
        }

        public int CompareTo(object obj)
        {
            RelayBufferMessage message = (RelayBufferMessage)obj;
            return Date.CompareTo(message.Date);
        }
    }
}
