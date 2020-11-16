using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;

namespace WinWeelay.Utils
{
    /// <summary>
    /// Utility class for text formatting helper methods.
    /// </summary>
    public static class FormattingUtils
    {
        /// <summary>
        /// Retrieve all fonts installed on the system.
        /// </summary>
        /// <returns>A list of font names.</returns>
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

        /// <summary>
        /// Remove all formatting codes from a WeeChat message.
        /// </summary>
        /// <param name="formattedString">The messages containing formatting codes.</param>
        /// <returns>A stripped version of the given message.</returns>
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
                    {
                        if (prefixChar == 'F')
                            isFirstFormattingCharacter = false;

                        continue;
                    }
                    isInFormatting = false;
                }

                if (!isInFormatting)
                    result += prefixChar;
            }

            return result;
        }
    }
}
