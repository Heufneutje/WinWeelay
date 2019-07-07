using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;

namespace WinWeelay.Utils
{
    public static class FormattingUtils
    {
        public static List<string> GetInstalledFonts()
        {
            List<string> fonts = new List<string>();
            using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
            {
                System.Drawing.FontFamily[] fontFamilies = fontsCollection.Families;
                foreach (System.Drawing.FontFamily font in fontFamilies.Where(x => !string.IsNullOrEmpty(x.Name)))
                    fonts.Add(font.Name);
            }

            return fonts;
        }

        public static string StripWeechatFormatting(string formattedString)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(formattedString))
                return result;

            bool isInFormatting = false;
            bool isFirstFormattingCharacter = false;

            foreach (char prefixChar in formattedString)
            {
                if (prefixChar == '\u0019' || prefixChar == '\u001a' || prefixChar == '\u001c')
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
