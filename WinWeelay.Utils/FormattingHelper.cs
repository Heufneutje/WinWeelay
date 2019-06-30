using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;

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

        public static List<string> GetInstalledFonts()
        {
            List<string> fonts = new List<string>();
            using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
            {
                FontFamily[] fontFamilies = fontsCollection.Families;
                foreach (FontFamily font in fontFamilies.Where(x => !string.IsNullOrEmpty(x.Name)))
                    fonts.Add(font.Name);
            }

            return fonts;
        }
    }
}
