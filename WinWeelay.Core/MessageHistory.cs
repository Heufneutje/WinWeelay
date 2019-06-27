using System.Collections.Generic;
using WinWeelay.Configuration;

namespace WinWeelay.Core
{
    public class MessageHistory
    {
        private List<string> _messageHistory;
        private RelayConfiguration _configuration;
        private int _historyIndex;

        public MessageHistory(RelayConfiguration configuration)
        {
            _messageHistory = new List<string>();
            _configuration = configuration;
            _historyIndex = -1;
        }

        public void AddHistoryEntry(string message)
        {
            _messageHistory.Add(message);
            if (_messageHistory.Count > _configuration.HistorySize)
                _messageHistory.RemoveAt(0);

            _historyIndex = -1;
        }

        public string GetPreviousHistoryEntry()
        {
            if (_historyIndex != _messageHistory.Count - 1)
                _historyIndex++;

            return GetHistoryEntry();
        }

        public string GetNextHistoryEntry()
        {
            if (_historyIndex > -1)
                _historyIndex--;

            return GetHistoryEntry();
        }

        private string GetHistoryEntry()
        {
            if (_historyIndex == -1)
                return string.Empty;

            return _messageHistory[_messageHistory.Count - 1 - _historyIndex];
        }
    }
}
