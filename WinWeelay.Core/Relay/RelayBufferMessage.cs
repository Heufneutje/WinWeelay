﻿using System;
using System.Collections.Generic;
using System.Linq;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    public class RelayBufferMessage
    {
        public string LinePointer { get; private set; }
        public string BufferPointer { get; private set; }
        public DateTime Date { get; private set; }
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
                    _unformattedPrefix = FormattingUtils.StripWeechatFormatting(Prefix);
                return _unformattedPrefix;
            }
        }

        private string _unformattedMessage;
        public string UnformattedMessage
        {
            get
            {
                if (_unformattedMessage == null)
                    _unformattedMessage = FormattingUtils.StripWeechatFormatting(Message);
                return _unformattedMessage;
            }
        }

        public RelayBufferMessage(WeechatHdataEntry entry, bool showNotifications, int linePointerIndex)
        {
            LinePointer = entry.GetPointer(linePointerIndex);
            BufferPointer = entry["buffer"].AsPointer();
            Date = entry["date"].AsTime();
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
                   LinePointer == message.LinePointer &&
                   BufferPointer == message.BufferPointer;
        }

        public override int GetHashCode()
        {
            int hashCode = 1910936884;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LinePointer);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BufferPointer);
            return hashCode;
        }

        public static bool operator ==(RelayBufferMessage left, RelayBufferMessage right)
        {
            return EqualityComparer<RelayBufferMessage>.Default.Equals(left, right);
        }

        public static bool operator !=(RelayBufferMessage left, RelayBufferMessage right)
        {
            return !(left == right);
        }
    }
}
