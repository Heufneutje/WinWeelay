using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WinWeelay
{
    public class BoolToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return FontWeights.Normal;

            return ((bool)value) ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is FontWeights))
                return false;

            return ((FontWeight)value) == FontWeights.Bold;
        }
    }
}
