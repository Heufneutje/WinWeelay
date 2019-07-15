using System;
using System.Windows.Media;

namespace WinWeelay
{
    public class ColorHelper
    {
        private int[] _basicColors;
        private int[] _extendedColors;
        private int[] _weechatColors;

        public ColorHelper()
        {
            _basicColors = new int[]{
                0x000000, // Black
                0x800000, // Red
                0x008000, // Green
                0x808000, // Light Yellow(Brown)
                0x000080, // Blue
                0x800080, // Magenta
                0x008080, // Light Cyan
                0xC0C0C0, // Light Gray
                0x808080, // Gray
                0xFF0000, // Light Red
                0x00FF00, // Light Green
                0xFFFF00, // Yellow
                0x0000FF, // Light Blue
                0xFF00FF, // Light Magenta
                0x00FFFF, // Cyan
                0xFFFFFF  // White
            };

            _extendedColors = new int[256];
            int[] colorBase = new int[] { 0x00, 0x5F, 0x87, 0xAF, 0xD7, 0xFF };
            for (int i = 16; i < 232; i++)
            {
                int j = i - 16;
                _extendedColors[i] = (colorBase[(j / 36) % 6]) << 16 | (colorBase[(j / 6) % 6] << 8 | (colorBase[j % 6]));
            }
            for (int i = 232; i < 256; i++)
            {
                int j = 8 + i * 10;
                _extendedColors[i] = j << 16 | j << 8 | j;
            }

            _weechatColors = new int[] {
                0,  // Default
                0,  // Black
                8,  // Dark Gray
                1,  // Red
                9,  // Light Red
                2,  // Green
                10, // Light Green
                3,  // Brown
                11, // Yellow
                4,  // Blue
                12, // Light Blue
                5,  // Magenta
                13, // Light Magenta
                6, // Light Cyan
                14,  // Cyan
                7,  // Gray
                15, // White
            };
        }

        public Color GetColor(int colorCode)
        {
            if (colorCode < 0)
                return default;
            if (colorCode < _basicColors.Length)
                return ConvertColor(_basicColors[colorCode]);
            if (colorCode < _extendedColors.Length)
                return ConvertColor(_extendedColors[colorCode]);
            return default;
        }

        public Color GetWeechatColor(int colorCode)
        {
            if (colorCode < 1 || colorCode > _weechatColors.Length)
                return default;
            return GetColor(_weechatColors[colorCode]);
        }

        public Color GetExtendedColor(int colorCode)
        {
            if (colorCode < 0 || colorCode > _extendedColors.Length)
                return default;

            return ConvertColor(_extendedColors[colorCode]);
        }

        public bool IsExtendedColor(int colorCode)
        {
            return colorCode > _basicColors.Length;
        }

        private Color ConvertColor(int colorCode)
        {
            byte[] values = BitConverter.GetBytes(colorCode);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(values);

            return Color.FromArgb(255, values[2], values[1], values[0]);
        }
    }
}
