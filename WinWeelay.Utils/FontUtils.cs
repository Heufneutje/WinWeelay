using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;

namespace WinWeelay.Utils
{
    /// <summary>
    /// Utility class for fonts.
    /// </summary>
    public static class FontUtils
    {
        /// <summary>
        /// Retrieve all fonts installed on the system.
        /// </summary>
        /// <returns>A list of font names.</returns>
        public static List<string> GetInstalledFonts()
        {
            List<string> fonts = new();
            using (InstalledFontCollection fontsCollection = new())
            {
                System.Drawing.FontFamily[] fontFamilies = fontsCollection.Families;
                foreach (System.Drawing.FontFamily font in fontFamilies.Where(x => !string.IsNullOrEmpty(x.Name)))
                    fonts.Add(font.Name);
            }

            return fonts;
        }
    }
}
