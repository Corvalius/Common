using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Enumerations
{
    public static class EnumerationExtensions40
    {
        public static bool TryGetDisplayName<T>(this T enumValue, out string displayName)
        {
            displayName = string.Empty;

            Type enumType = enumValue.GetType();
            if (!enumType.IsEnum)
                return false;

            return TryGetDisplayName(enumType, enumValue, out displayName);
        }

        public static string GetDisplayName<T>(this T enumValue)
        {
            Type enumType = enumValue.GetType();
            if (!enumType.IsEnum)
                throw new ArgumentException("Type " + enumType.Name + " is not an enum");

            string displayName;
            if (!TryGetDisplayName(enumType, enumValue, out displayName))
                throw new ArgumentException("Type " + enumType.Name + " is not an enum");

            return displayName;
        }

        public static bool TryGetDisplayName<T>(this Type enumType, T enumValue, out string displayName)
        {
            displayName = string.Empty;
            if (!enumType.IsEnum)
                return false;

            var values = enumType.GetKeyValues();
            if (values.ContainsKey(enumValue))
            {
                displayName = values[enumValue];
                return true;
            }

            return false;
        }

        public static Dictionary<object, string> GetKeyValues(this Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type " + enumType.Name + " is not an enum");
            }

            var values = new Dictionary<object, string>();

            var fields = enumType.GetLiteralFields();

            foreach (FieldInfo field in fields)
            {
                object key = field.GetValue(enumType);
                string displayName = field.GetDisplayName(key.ToString());

                values.Add(key, displayName);
            }
            return values;
        }

        private static void CheckIsEnum<T>(bool withFlags)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(string.Format("Type '{0}' is not an enum", typeof(T).FullName));
            if (withFlags && !Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
                throw new ArgumentException(string.Format("Type '{0}' doesn't have the 'Flags' attribute", typeof(T).FullName));
        }

        public static IEnumerable<T> GetFlags<T>(this T value) where T : struct
        {
            CheckIsEnum<T>(true);
            foreach (T flag in Enum.GetValues(typeof(T)).Cast<T>())
            {
                if (value.IsFlagSet(flag))
                    yield return flag;
            }
        }

        public static string GetDescription<T>(this T value) where T : struct
        {
            CheckIsEnum<T>(false);
            string name = Enum.GetName(typeof(T), value);
            if (name != null)
            {
                FieldInfo field = typeof(T).GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
    }
}