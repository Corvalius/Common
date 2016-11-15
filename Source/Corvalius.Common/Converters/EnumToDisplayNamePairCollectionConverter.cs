using System;
using System.Enumerations;
using System.Windows.Data;

namespace Corvalius.Converters
{
    /// <summary>
    /// This converter is similar to the PairConverter,
    /// except that given a single enum value, 
    /// it will return a dictionary of values and their matching display names.
    /// </summary>
    public sealed class EnumToDisplayNamePairCollectionConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var keyValues = value.GetType().GetKeyValues();
                return keyValues;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
