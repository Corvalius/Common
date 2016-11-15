using System;
using System.Windows;
using System.Windows.Data;

namespace Corvalius.Converters
{
    public sealed class ObjectToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool reverse = false;
            bool result = value == null;

            if (parameter != null)
                reverse = true;

            if (reverse)
                result = !result;

            if (result)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
