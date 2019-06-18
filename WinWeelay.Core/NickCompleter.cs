using System;
using System.Collections.Generic;
using System.Linq;
using WinWeelay.Utils;

namespace WinWeelay.Core
{
    public class NickCompleter
    {
        private RelayBuffer _buffer;
        private int _nickCompleteIndex;
        private string _lastNickCompletion;
        private string _lastSearch;

        public bool IsNickCompleting { get; set; }

        public NickCompleter(RelayBuffer buffer)
        {
            _buffer = buffer;
            _nickCompleteIndex = -1;
        }

        public string HandleNickCompletion(string message)
        {
            _nickCompleteIndex++;
            string lastWord = string.Empty;
            bool isOnlyWord = true;

            if (!string.IsNullOrEmpty(message))
            {
                string[] words = message.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                lastWord = _lastSearch ?? words.Last();
                isOnlyWord = words.Length == 1;
            }

            _lastSearch = lastWord;

            string completedNick = GetCompletedNick(lastWord);
            if (completedNick != null)
            {
                if (isOnlyWord)
                    completedNick = $"{completedNick}: ";

                if (string.IsNullOrEmpty(lastWord))
                    message = completedNick;
                else
                    message = message.ReplaceLastOccurrence(_lastNickCompletion ?? lastWord, completedNick);

                _lastNickCompletion = completedNick;

                return message;
            }

            return message;
        }

        private string GetCompletedNick(string message)
        {
            IEnumerable<string> sortedNicks = _buffer.GetSortedUniqueNicks();

            if (!string.IsNullOrWhiteSpace(message))
            {
                string lastWord = message.Split(' ').Last();
                sortedNicks = sortedNicks.Where(x => x.ToLower().StartsWith(lastWord.ToLower()));
            }

            if (!sortedNicks.Any())
                return null;

            if (_nickCompleteIndex > sortedNicks.Count() - 1)
                _nickCompleteIndex = 0;

            return sortedNicks.ElementAt(_nickCompleteIndex);
        }

        public void Reset()
        {
            _nickCompleteIndex = -1;
            _lastNickCompletion = null;
            _lastSearch = null;
        }
    }
}
