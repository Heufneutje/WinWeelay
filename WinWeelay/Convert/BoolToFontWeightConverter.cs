using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WinWeelay
{
    /// <summary>
    /// Converter to map boolean values to bold or normal font weight (true = bold, false = normal).
    /// </summary>
    public class BoolToFontWeightConverter : IValueConverter
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
                return FontWeights.Normal;

            return ((bool)value) ? FontWeights.Bold : FontWeights.Normal;
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
            if (!(value is FontWeight))
                return false;

            return ((FontWeight)value) == FontWeights.Bold;
        }
    }
}
