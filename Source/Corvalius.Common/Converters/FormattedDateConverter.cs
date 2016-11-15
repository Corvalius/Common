using System;
using System.Windows.Data;

namespace Corvalius.Converters
{
    /// <summary>
    /// Convertes a DateTime into a short date string by default, or with a provided date formating string.
    /// </summary>
    public sealed class FormattedDateConverter : IValueConverter
    {
        public string Format { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is DateTime))
                return "No Date";

            var date = (DateTime)value;

            if (this.Format == null)
                return date.ToShortDateString();

            return date.ToString(this.Format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string date = value.ToString();
                DateTime dateTime;
                if (DateTime.TryParse(date, out dateTime))
                {
                    return dateTime;
                }
            }

            return null;
        }

        #endregion
    }
}
