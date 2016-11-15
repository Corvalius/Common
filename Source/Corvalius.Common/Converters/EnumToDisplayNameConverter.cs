using System;
using System.Enumerations;
using System.Windows.Data;

namespace Corvalius.Converters
{
    /// <summary>
    /// This converter will return the display name of an enum value if it has been annotated like so:
    /// 
    /// public enum NumberType
    /// {
    ///     [Display(Name = "Positive")]
    ///     PositiveNumber,
    ///     
    ///     [Display(Name = "Negative")]
    ///     NegativeNumber
    /// }
    /// 
    /// If it has not been annotated, then the name of the enum value is used.
    /// </summary>
    public sealed class EnumToDisplayNameConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string displayName;
                if (value.TryGetDisplayName(out displayName))
                {
                    return displayName;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
