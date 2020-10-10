using System;
using System.Globalization;
using System.Windows.Data;

namespace WinWeelay
{
    /// <summary>
    /// Converter to map boolean values to WeeChat booleans (true = on, false = off).
    /// </summary>
    public class BoolToOptionStringConverter : IValueConverter
    {
        /// <summary>
        /// IValueConverter implementation.
        /// </summary>
        /// <param name="value">IValueConverter implementation.</param>
        /// <param name="targetType">IValueConverter implementation.</param>
        /// <param name="parameter">IValueConverter implementation.</param>
        /// <param name="culture">IValueConverter implementation.</param>
        /// <returns>IValueConverter implementation.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
                return false;

            return ((string)value) == "on";
        }

        /// <summary>
        /// IValueConverter implementation.
        /// </summary>
        /// <param name="value">IValueConverter implementation.</param>
        /// <param name="targetType">IValueConverter implementation.</param>
        /// <param name="parameter">IValueConverter implementation.</param>
        /// <param name="culture">IValueConverter implementation.</param>
        /// <returns>IValueConverter implementation.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return "off";

            return ((bool)value) ? "on" : "off";
        }
    }
}
