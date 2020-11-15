using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace WinWeelay.CustomControls
{
    /// <summary>
    /// Color for IRC formatting.
    /// </summary>
    public class IrcColor
    {
        /// <summary>
        /// IRC color value.
        /// </summary>
        public byte ColorIndex { get; set; }

        /// <summary>
        /// Decsription of the color.
        /// </summary>
        public string ColorName { get; set; }

        /// <summary>
        /// WPF color.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Brush for using the color within controls.
        /// </summary>
        public Brush ColorBrush => new SolidColorBrush(Color);

        /// <summary>
        /// Create a new instance of a color
        /// </summary>
        /// <param name="colorIndex">IRC color value.</param>
        /// <param name="color">WPF color.</param>
        /// <param name="colorName">Decsription of the color.</param>
        public IrcColor(byte colorIndex, Color color, string colorName)
        {
            ColorIndex = colorIndex;
            Color = color;
            ColorName = colorName;
        }

        private static List<IrcColor> _colors;

        /// <summary>
        /// Get a list of all available IRC colors.
        /// </summary>
        /// <returns>A list of IRC colors.</returns>
        public static List<IrcColor> GetColors()
        {
            if (_colors == null)
                _colors = new List<IrcColor>()
                {
                    new IrcColor(99, Color.FromArgb(0, 0, 0, 0), "Default"),
                    new IrcColor(0, Color.FromArgb(255, 255, 255, 255), "White"),
                    new IrcColor(1, Color.FromArgb(255, 0, 0, 0), "Black"),
                    new IrcColor(2, Color.FromArgb(255, 0, 0, 127), "Blue"),
                    new IrcColor(3, Color.FromArgb(255, 0, 147, 0), "Green"),
                    new IrcColor(4, Color.FromArgb(255, 255, 0, 0), "Light Red"),
                    new IrcColor(5, Color.FromArgb(255, 127, 0, 0), "Brown"),
                    new IrcColor(6, Color.FromArgb(255, 156, 0, 156), "Purple"),
                    new IrcColor(7, Color.FromArgb(255, 255, 127, 0), "Orange"),
                    new IrcColor(8, Color.FromArgb(255, 255, 255, 0), "Yellow"),
                    new IrcColor(9, Color.FromArgb(255, 252, 0, 0), "Light Green"),
                    new IrcColor(10, Color.FromArgb(255, 0, 147, 147), "Cyan"),
                    new IrcColor(11, Color.FromArgb(255, 0, 255, 255), "Light Cyan"),
                    new IrcColor(12, Color.FromArgb(255, 0, 0, 252), "Light Blue"),
                    new IrcColor(13, Color.FromArgb(255, 255, 0, 255), "Pink"),
                    new IrcColor(14, Color.FromArgb(255, 127, 127, 127), "Grey"),
                    new IrcColor(15, Color.FromArgb(255, 210, 210, 210), "Light Grey")
                };

            return _colors;
        }

        /// <summary>
        /// Get the IRC color code for a given color.
        /// </summary>
        /// <param name="color">A WPF color.</param>
        /// <returns>THe IRC color code.</returns>
        public static byte GetColorCode(Color color)
        {
            byte? colorCode = GetColors().FirstOrDefault(x => x.Color == color)?.ColorIndex;
            if (colorCode == null)
                colorCode = 99;

            return colorCode.Value;
        }

        /// <summary>
        /// Override to match colors by their color code.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if color codes match.</returns>
        public override bool Equals(object obj)
        {
            return obj is IrcColor color &&
                   ColorIndex == color.ColorIndex;
        }

        /// <summary>
        /// Override to match colors by their color code.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return 1516799092 + ColorIndex.GetHashCode();
        }

        /// <summary>
        /// Override to match colors by their color code.
        /// </summary>
        /// <param name="left">First color.</param>
        /// <param name="right">Second color.</param>
        /// <returns>True if color codes match.</returns>
        public static bool operator ==(IrcColor left, IrcColor right)
        {
            return EqualityComparer<IrcColor>.Default.Equals(left, right);
        }

        /// <summary>
        /// Override to match colors by their color code.
        /// </summary>
        /// <param name="left">First color.</param>
        /// <param name="right">Second color.</param>
        /// <returns>False if color codes match.</returns>
        public static bool operator !=(IrcColor left, IrcColor right)
        {
            return !(left == right);
        }
    }
}
