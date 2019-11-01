namespace WinWeelay.Utils
{
    /// <summary>
    /// Extension class for string operations.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Replace the last occurrence of a word in a given string.
        /// </summary>
        /// <param name="source">The string to check.</param>
        /// <param name="wordToReplace">The word to replace.</param>
        /// <param name="replacement">The replacement for the given word.</param>
        /// <returns></returns>
        public static string ReplaceLastOccurrence(this string source, string wordToReplace, string replacement)
        {
            int place = source.LastIndexOf(wordToReplace);

            if (place == -1)
                return source;

            string result = source.Remove(place, wordToReplace.Length).Insert(place, replacement);
            return result;
        }
    }
}
