namespace WinWeelay.Utils
{
    public static class StringExtension
    {
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
