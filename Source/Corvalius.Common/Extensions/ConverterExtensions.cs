using System.Globalization;
using System.Windows.Data;

namespace Corvalius
{
    public static class ConverterExtensions
    {
        public static T Convert<T>(this IValueConverter converter, object value)
        {
            try
            {
                return (T)converter.Convert(value, typeof(T), null, CultureInfo.CurrentCulture);
            }
            catch
            {
                return default(T);
            }
        }
    }
}
