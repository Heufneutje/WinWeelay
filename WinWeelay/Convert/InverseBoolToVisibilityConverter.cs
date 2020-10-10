using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WinWeelay
{
    /// <summary>
    /// Converter to map boolean values to UI visibility (true = collapsed, false = visible).
    /// </summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
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
                return Visibility.Collapsed;

            return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
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
            if (!(value is Visibility))
                return true;

            return ((Visibility)value) == Visibility.Collapsed;
        }
    }
}
