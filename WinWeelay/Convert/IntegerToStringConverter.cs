using System;
using System.Globalization;
using System.Windows.Data;

namespace WinWeelay
{
    public class IntegerToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                return 0;

            return System.Convert.ToInt32(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int))
                return "0";

            return value.ToString();
        }
    }
}
