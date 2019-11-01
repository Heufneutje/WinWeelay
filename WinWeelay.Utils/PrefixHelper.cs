namespace WinWeelay.Utils
{
    /// <summary>
    /// Utlity class for sorting IRC prefixes.
    /// </summary>
    public static class PrefixHelper
    {
        /// <summary>
        /// Return a sort index based on the common !&amp;@%+ prefixes.
        /// </summary>
        /// <param name="prefix">The prefix to check the order of.</param>
        /// <returns>An index representing the prefix order.</returns>
        public static int GetSortIndex(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                return 5;

            if (prefix.Contains("~"))
                return 0;
            else if (prefix.Contains("&"))
                return 1;
            else if (prefix.Contains("@"))
                return 2;
            else if (prefix.Contains("%"))
                return 3;
            else if (prefix.Contains("+"))
                return 4;

            return 5;
        }
    }
}
