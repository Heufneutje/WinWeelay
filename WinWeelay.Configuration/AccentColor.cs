using System;
using System.Collections.Generic;
using WinWeelay.Utils;

namespace WinWeelay.Configuration
{
    /// <summary>
    /// Color values for the accent color in the UI.
    /// </summary>
    [Serializable]
    public class AccentColor : BaseChangeTrackable
    {
        /// <summary>
        /// Red byte value.
        /// </summary>
        public byte RedValue { get; set; }

        /// <summary>
        /// Green byte value.
        /// </summary>
        public byte GreenValue { get; set; }

        /// <summary>
        /// Blue byte value.
        /// </summary>
        public byte BlueValue { get; set; }

        /// <summary>
        /// Empty constructor for designer binding.
        /// </summary>
        public AccentColor() { }

        /// <summary>
        /// Create a new accent color from color bytes.
        /// </summary>
        /// <param name="redValue">Red byte value.</param>
        /// <param name="greenValue">Green byte value.</param>
        /// <param name="blueValue">Blue byte value.</param>
        public AccentColor(byte redValue, byte greenValue, byte blueValue)
        {
            RedValue = redValue;
            GreenValue = greenValue;
            BlueValue = blueValue;
        }

        /// <summary>
        /// Override to make objects match if their color bytes are the same.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if color values match.</returns>
        public override bool Equals(object obj)
        {
            return obj is AccentColor color &&
                   RedValue == color.RedValue &&
                   GreenValue == color.GreenValue &&
                   BlueValue == color.BlueValue;
        }

        /// <summary>
        /// Override to make objects match if their color bytes are the same.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            int hashCode = -1641718444;
            hashCode = hashCode * -1521134295 + RedValue.GetHashCode();
            hashCode = hashCode * -1521134295 + GreenValue.GetHashCode();
            hashCode = hashCode * -1521134295 + BlueValue.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Operator to make objects match if their color bytes are the same.
        /// </summary>
        /// <param name="left">First color to check.</param>
        /// <param name="right">Second color to check.</param>
        /// <returns>True if color values match.</returns>
        public static bool operator ==(AccentColor left, AccentColor right)
        {
            return EqualityComparer<AccentColor>.Default.Equals(left, right);
        }

        /// <summary>
        /// Operator to make objects match if their color bytes are the same.
        /// </summary>
        /// <param name="left">First color to check.</param>
        /// <param name="right">Second color to check.</param>
        /// <returns>True if color values do not match.</returns>
        public static bool operator !=(AccentColor left, AccentColor right)
        {
            return !(left == right);
        }
    }
}
