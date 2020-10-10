using System;
using System.Globalization;
using System.Windows.Data;

namespace WinWeelay
{
    /// <summary>
    /// Converter to invert boolean values.
    /// </summary>
    public class InverseBoolConverter : IValueConverter
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
            if (!(value is bool))
                return false;

            return !(bool)value;
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
            return Convert(value, targetType, parameter, culture);
        }
    }
}
