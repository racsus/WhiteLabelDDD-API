using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace WhiteLabel.Infrastructure.Data.Extensions
{
    /// <summary>
    /// Extension for System.String
    /// </summary>
    public static class StringExtensions
    {
        public static readonly MethodInfo StartsWithMethod = ExpressionExtensions.GetMethodInfo(
            typeof(string),
            "StartsWith",
            1
        );

        public static readonly MethodInfo EndsWithMethod = ExpressionExtensions.GetMethodInfo(
            typeof(string),
            "EndsWith",
            1
        );

        public static readonly MethodInfo ContainsMethod = ExpressionExtensions.GetMethodInfo(
            typeof(string),
            "Contains",
            1
        );

        /// <summary>
        /// Converts a string into an object of the specified type
        /// </summary>
        /// <param name="s">String</param>
        /// <param name="type">Conversion type</param>
        /// <returns>Object in <paramref name="type"/> type with the String value if its possible</returns>
        public static object ConvertTo(this string s, Type type)
        {
            var targetType = type.GetEffectiveType();

            if (targetType == typeof(string))
                return s;
            if (targetType == typeof(Guid))
                return Guid.Parse(s);
            if (s.IsNullOrEmpty())
                return null;

            if (targetType.IsEnum())
                return s.ToEnum(targetType, null);

            return Convert.ChangeType(s, targetType);
        }

        /// <summary>
        /// Checks if a String is null, empty or consists only of white-spaces characters
        /// </summary>
        /// String
        /// <returns>True if
        ///     is null, empty or consists only of white-spaces characters if not returns false
        /// </returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object
        /// </summary>
        /// <typeparam name="T">EnumType</typeparam>
        /// <param name="value">String</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>An object of type <typeparamref name="T"/> whose value is represented by <paramref name="value"/></returns>
        public static T ToEnum<T>(this string value, T defaultValue)
            where T : struct
        {
            var type = typeof(T);
            if (!type.IsEnum())
                return defaultValue;
            var names = Enum.GetNames(type);
            if (names.Contains(value))
                return (T)Enum.Parse(type, value, true);
            var res = Enum.GetValues(type)
                .OfType<T>()
                .FirstOrDefault(
                    v => ((int)(object)v).ToString(CultureInfo.InvariantCulture) == value
                );

            return !Equals(res, default(T)) ? res : defaultValue;
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object
        /// </summary>
        /// <param name="value">String</param>
        /// <param name="type">EnumType</param>
        /// <param name="defaultValue">DefaultValue</param>
        /// <returns>An object of type</returns>
        private static object ToEnum(this string value, Type type, object defaultValue)
        {
            if (!type.IsEnum())
                return defaultValue;
            var names = Enum.GetNames(type);
            if (names.Contains(value))
                return Enum.Parse(type, value, true);
            foreach (var v in Enum.GetValues(type))
                if (((int)v).ToString(CultureInfo.InvariantCulture) == value)
                    return v;

            return defaultValue;
        }
    }
}
