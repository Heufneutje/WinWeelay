using System.Collections.Generic;
using WinWeelay.Configuration;

namespace WinWeelay.Core
{
    /// <summary>
    /// Stores the history of a buffer's input box.
    /// </summary>
    public class MessageHistory
    {
        private List<string> _messageHistory;
        private RelayConfiguration _configuration;
        private int _historyIndex;

        /// <summary>
        /// Create a new history instance.
        /// </summary>
        /// <param name="configuration">The main configuration.</param>
        public MessageHistory(RelayConfiguration configuration)
        {
            _messageHistory = new List<string>();
            _configuration = configuration;
            _historyIndex = -1;
        }

        /// <summary>
        /// Add a sent message to the message history.
        /// </summary>
        /// <param name="message">The message to add.</param>
        public void AddHistoryEntry(string message)
        {
            _messageHistory.Add(message);
            if (_messageHistory.Count > _configuration.HistorySize)
                _messageHistory.RemoveAt(0);

            _historyIndex = -1;
        }

        /// <summary>
        /// Get the message before the current index.
        /// </summary>
        /// <returns>A message from the history.</returns>
        public string GetPreviousHistoryEntry()
        {
            if (_historyIndex != _messageHistory.Count - 1)
                _historyIndex++;

            return GetHistoryEntry();
        }

        /// <summary>
        /// Get the message after the current index.
        /// </summary>
        /// <returns>A message from the history.</returns>
        public string GetNextHistoryEntry()
        {
            if (_historyIndex > -1)
                _historyIndex--;

            return GetHistoryEntry();
        }

        /// <summary>
        /// Clear all history entries.
        /// </summary>
        public void ClearHistory()
        {
            _historyIndex = -1;
            _messageHistory.Clear();
        }

        private string GetHistoryEntry()
        {
            if (_historyIndex == -1)
                return string.Empty;

            return _messageHistory[_messageHistory.Count - 1 - _historyIndex];
        }
    }
}
