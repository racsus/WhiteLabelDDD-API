using System;
using System.Collections.Generic;

namespace WhiteLabel.Infrastructure.Data.Extensions
{
    /// <summary>
    /// Extension of System.Type
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Set of numeric types
        /// </summary>
        private static readonly ISet<Type> NumericTypes = new HashSet<Type>(
            new[]
            {
                typeof(sbyte),
                typeof(byte),
                typeof(decimal),
                typeof(double),
                typeof(float),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(ushort),
                typeof(uint),
                typeof(ulong),
            }
        );

        /// <summary>
        /// Determine if a type is an enumeration.
        /// </summary>
        /// <param name="clrType">The type to test.</param>
        /// <returns>True if the type is an enumeration; false otherwise.</returns>
        public static bool Enum(this Type clrType)
        {
            var underlyingTypeOrSelf = GetUnderlyingTypeOrSelf(clrType);
            return underlyingTypeOrSelf.IsEnum;
        }

        private static Type GetUnderlyingTypeOrSelf(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        public static Type GetEffectiveType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        /// <summary>
        ///     Determines if a type is numeric.  Nullable numeric types are considered numeric.
        /// </summary>
        /// <remarks>
        ///     Boolean is not considered numeric.
        /// </remarks>
        public static bool NumericType(this Type type)
        {
            if (type == null)
                return false;

            var effectiveType = type.GetEffectiveType();

            return !effectiveType.Enum() && NumericTypes.Contains(effectiveType);
        }
    }
}
