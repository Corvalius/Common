using System;
using System.Collections.Generic;
using System.Enumerations;
using System.Globalization;
using System.Windows.Data;

namespace Corvalius.Converters
{
    /// <summary>
    /// Converts an Enum value into a KeyValuePair 
    /// with the key being the actual enum value passed in,
    /// and the value being the display name of that value.
    ///
    /// This converter allows each value to be annotated like so:
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
    /// This will result in:
    /// Key:PositiveNumber Value:"Positive" or
    /// Key:NegativeNumber Value:"Negative"
    /// 
    /// It will default to the name of the enum value if there is no display attribute applied.
    /// </summary>
    public sealed class EnumToDisplayNamePairConverter : IValueConverter
    {
        private KeyValuePair<object, string> lastGoodValue;

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value != null)
                {
                    Type valueType = value.GetType();
                    object enumValue = value;
                    
                    string displayName;
                    if (!valueType.TryGetDisplayName(value, out displayName))
                    {
                        enumValue = valueType.GetDefaultValue();
                        enumValue.TryGetDisplayName(out displayName);
                    }

                    var pair = new KeyValuePair<object, string>(enumValue, displayName);
                    lastGoodValue = pair;
                    return pair;
                }

                return lastGoodValue;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var pair = (KeyValuePair<object, string>)value;
                lastGoodValue = pair;
                return pair.Key;
            }

            return lastGoodValue.Key;
        }

        #endregion
    }
}
