using System;
using System.Collections.Generic;
using System.Text;
using WinWeeRelay.Utils;

namespace WinWeeRelay.Core
{
    public class RelayBufferMessage
    {
        public string BufferPointer { get; private set; }
        public DateTime Date { get; private set; }
        public DateTime DatePrinted { get; private set; }
        public bool IsDisplayed { get; private set; }
        public bool IsHighlighted { get; private set; }
        public string[] Tags { get; private set; }
        public string Prefix { get; private set; }
        public string Message { get; private set; }

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

        public RelayBufferMessage(WeechatHdataEntry entry)
        {
            BufferPointer = entry["buffer"].AsPointer();
            Date = entry["date"].AsTime();
            DatePrinted = entry["date_printed"].AsTime();
            IsDisplayed = Convert.ToBoolean(entry["displayed"].AsBoolean());
            IsHighlighted = Convert.ToBoolean(entry["highlight"].AsBoolean());
            Tags = entry["tags_array"].AsArray().ToStringArray();
            Prefix = entry["prefix"].AsString();
            Message = entry["message"].AsString();
        }

        public override string ToString()
        {
            return $"{Date:HH:mm:ss} <{UnformattedPrefix}> {UnformattedMessage}";
        }
    }
}
