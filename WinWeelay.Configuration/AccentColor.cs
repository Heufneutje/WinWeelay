using System;
using System.Collections.Generic;
using WinWeelay.Utils;

namespace WinWeelay.Configuration
{
    [Serializable]
    public class AccentColor : BaseChangeTrackable
    {
        public byte RedValue { get; set; }
        public byte GreenValue { get; set; }
        public byte BlueValue { get; set; }

        public AccentColor() { }

        public AccentColor(byte redValue, byte greenValue, byte blueValue)
        {
            RedValue = redValue;
            GreenValue = greenValue;
            BlueValue = blueValue;
        }

        public override bool Equals(object obj)
        {
            return obj is AccentColor color &&
                   RedValue == color.RedValue &&
                   GreenValue == color.GreenValue &&
                   BlueValue == color.BlueValue;
        }

        public override int GetHashCode()
        {
            int hashCode = -1641718444;
            hashCode = hashCode * -1521134295 + RedValue.GetHashCode();
            hashCode = hashCode * -1521134295 + GreenValue.GetHashCode();
            hashCode = hashCode * -1521134295 + BlueValue.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(AccentColor left, AccentColor right)
        {
            return EqualityComparer<AccentColor>.Default.Equals(left, right);
        }

        public static bool operator !=(AccentColor left, AccentColor right)
        {
            return !(left == right);
        }
    }
}
