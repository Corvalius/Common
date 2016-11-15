using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace System
{
    public static class TypeExtensions
    {
        public static IEnumerable<FieldInfo> GetLiteralFields(this Type type)
        {
            var fields = from field in type.GetFields()
                         where field.IsLiteral
                         select field;
            return fields;
        }

        public static string GetDisplayName(this FieldInfo field, string defaultValue)
        {
            string displayName = string.Empty;

            var customAttributes = field.GetCustomAttributes(false);
            var displayAttribute = customAttributes.Where(a => a is DisplayAttribute).SingleOrDefault() as DisplayAttribute;

            if (displayAttribute != null)
            {
                displayName = displayAttribute.Name;
            }
            else
            {
                displayName = defaultValue;
            }

            return displayName;
        }

        public static T GetDefaultValue<T>(this T value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (T)(value.GetType().GetDefaultValue());
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            var customAttributes = type.GetCustomAttributes(false);

            if (!customAttributes.IsNullOrEmpty())
            {
                var attribute = customAttributes.Where(o => o is DefaultValueAttribute).SingleOrDefault();

                if (attribute != null)
                {
                    var defaultValue = attribute as DefaultValueAttribute;
                    if (defaultValue != null)
                        return defaultValue.Value;
                }
            }

            return null;
        }
    }
}
