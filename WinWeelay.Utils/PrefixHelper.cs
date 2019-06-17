namespace WinWeelay.Utils
{
    public static class PrefixHelper
    {
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
