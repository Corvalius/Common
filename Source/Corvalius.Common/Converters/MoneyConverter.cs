using System;
using System.Windows.Data;

namespace Corvalius.Converters
{
    /// <summary>
    /// Converts a number value into a currency string and back into a double.
    /// </summary>
    public sealed class MoneyConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double val;
            if (value != null)
            {
                if (double.TryParse(value.ToString(), out val))
                {
                    return val.ToString("C");
                }
            }

            return "$0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string sval = value.ToString();
                double val;
                if (double.TryParse(
                        sval,
                        System.Globalization.NumberStyles.AllowCurrencySymbol |
                        System.Globalization.NumberStyles.AllowThousands |
                        System.Globalization.NumberStyles.AllowDecimalPoint,
                        null,
                        out val))
                {
                    return val;
                }
            }

            return 0.0;
        }

        #endregion
    }
}
