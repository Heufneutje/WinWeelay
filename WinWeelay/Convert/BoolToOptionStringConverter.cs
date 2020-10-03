using System;
using System.Globalization;
using System.Windows.Data;

namespace WinWeelay
{
    public class BoolToOptionStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                return false;

            return ((string)value) == "on";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return "off";

            return ((bool)value) ? "on" : "off";
        }
    }
}
