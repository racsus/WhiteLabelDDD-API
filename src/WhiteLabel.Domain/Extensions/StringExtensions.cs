﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace WhiteLabel.Domain.Extensions
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
        /// <typeparam name="T">Conversion type</typeparam>
        /// <param name="s">String</param>
        /// <returns>Object in <typeparamref name="T"/> type with the String value if its possible</returns>
        public static T ConvertTo<T>(this string s)
        {
            var targetType = typeof(T).GetEffectiveType();

            if (targetType == typeof(string))
            {
                return (T)(object)s;
            }
            if (s.IsNullOrEmpty())
            {
                return default(T);
            }

            if (targetType.IsEnum())
            {
                return (T)s.ToEnum(targetType, default(T));
            }

            return (T)Convert.ChangeType(s, targetType);
        }

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
            {
                return s;
            }
            if (s.IsNullOrEmpty())
            {
                return null;
            }

            if (targetType.IsEnum())
            {
                return s.ToEnum(targetType, null);
            }

            return Convert.ChangeType(s, targetType);
        }

        /// <summary>
        /// Checks if a String is null, empty or consists only of white-spaces characters
        /// </summary>
        /// <param name="s">String</param>
        /// <returns>True if <param name="s">is null, empty or consists only of white-spaces characters if not returns false</returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// Concatenates the elements of a specified array or the members of a collection, using the specified separator between each element or member
        /// </summary>
        /// <param name="s">String collection</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> s, string separator)
        {
            return string.Join(separator, s);
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object
        /// </summary>
        /// <typeparam name="T">EnumType</typeparam>
        /// <param name="value">String</param>
        /// <returns>An object of type <typeparamref name="T"/> whose value is represented by <paramref name="value"/></returns>
        public static T ToEnum<T>(this string value)
            where T : struct
        {
            return ToEnum(value, default(T));
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
            {
                return defaultValue;
            }
            var names = Enum.GetNames(type);
            if (names.Contains(value))
            {
                return (T)Enum.Parse(type, value, true);
            }
            var res = Enum.GetValues(type)
                .OfType<T>()
                .FirstOrDefault(v =>
                    ((int)(object)v).ToString(CultureInfo.InvariantCulture) == value
                );

            return !Equals(res, default(T)) ? res : defaultValue;
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object
        /// </summary>
        /// <param name="value">String</param>
        /// <param name="type">EnumType</param>
        /// <param name="defaultValue">DefaultValue</param>
        /// <returns>An object of type <typeparamref name="T"/> whose value is represented by <paramref name="value"/></returns>
        public static object ToEnum(this string value, Type type, object defaultValue)
        {
            if (!type.IsEnum())
            {
                return defaultValue;
            }
            var names = Enum.GetNames(type);
            if (names.Contains(value))
            {
                return Enum.Parse(type, value, true);
            }
            foreach (var v in Enum.GetValues(type))
            {
                if (((int)v).ToString(CultureInfo.InvariantCulture) == value)
                {
                    return v;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Converts the first character of the string to lowercase
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>A copy of the string with the first character with the equivalent lowercase</returns>
        public static string FirstCharacterToLower(this string str)
        {
            if (string.IsNullOrEmpty(str) || char.IsLower(str, 0))
                return str;

            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// Converts the first character of the string to uppercase
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>A copy of the string with the first character with the equivalent uppercase</returns>
        public static string FirstCharacterToUpper(this string str)
        {
            if (string.IsNullOrEmpty(str) || char.IsUpper(str, 0))
                return str;

            return char.ToUpperInvariant(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// Encrypts a string using SHA1 algorithm
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Encrypted string</returns>
        public static string ToSHA1(this string str)
        {
            var sha1 = SHA1.Create();
            var encoding = new ASCIIEncoding();

            var sb = new StringBuilder();
            var stream = sha1.ComputeHash(encoding.GetBytes(str));

            for (int i = 0; i < stream.Length; i++)
            {
                sb.AppendFormat("{0:x2}", stream[i]);
            }
            return sb.ToString();
        }
    }
}
