namespace WinWeelay.Utils
{
    public static class FormattingHelper
    {
        public static string StripWeechatFormatting(string formattedString)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(formattedString))
                return result;

            bool isInFormatting = false;
            bool isFirstFormattingCharacter = false;

            foreach (char prefixChar in formattedString)
            {
                if (prefixChar == '\u0019')
                {
                    isInFormatting = true;
                    isFirstFormattingCharacter = true;
                }
                else if (isInFormatting)
                {
                    if (char.IsDigit(prefixChar) || isFirstFormattingCharacter && prefixChar == 'F' || prefixChar == '@')
                        continue;

                    isInFormatting = false;
                }

                if (!isInFormatting)
                    result += prefixChar;
            }

            return result;
        }
    }
}
