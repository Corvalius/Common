using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Corvalius.Converters
{
    public class ByteArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var byteArray = value as byte[];

            if (byteArray != null)
            {
                try
                {
                    return Encoding.Default.GetString(byteArray); //ToDo: Unicode was not working
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var @string = value as string;

            if (!string.IsNullOrWhiteSpace(@string))
            {
                try
                {
                    return Encoding.Default.GetBytes(@string); //ToDo: Unicode was not working
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }
}
