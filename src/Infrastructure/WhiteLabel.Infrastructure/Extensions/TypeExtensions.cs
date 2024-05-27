using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                typeof(ulong)
            }
        );

        /// <summary>
        /// Determine if a type is an enumeration.
        /// </summary>
        /// <param name="clrType">The type to test.</param>
        /// <returns>True if the type is an enumeration; false otherwise.</returns>
        public static bool IsEnum(this Type clrType)
        {
            var underlyingTypeOrSelf = GetUnderlyingTypeOrSelf(clrType);
            return underlyingTypeOrSelf.IsEnum;
        }

        public static Type GetUnderlyingTypeOrSelf(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        /// <summary>
        /// Determine if a type is a generic type.
        /// </summary>
        /// <param name="clrType">The type to test.</param>
        /// <returns>True if the type is a generic type; false otherwise.</returns>
        public static bool IsGenericType(this Type clrType)
        {
            return clrType.IsGenericType;
        }

        public static Type GetEffectiveType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        public static MethodInfo[] GetMethodsExtended(this Type type)
        {
            var current = type.GetMethods();
            var parentMethods = type.GetInterfaces()
                .SelectMany(it => it.GetMethodsExtended())
                .ToArray();
            var result = current.Union(parentMethods).Distinct().ToArray();
            return result;
        }

        public static PropertyInfo[] GetPropertiesExtended(this Type type)
        {
            var current = type.GetProperties();
            var parentProperties = type.GetInterfaces()
                .SelectMany(it => it.GetPropertiesExtended())
                .ToArray();
            var result = current.Union(parentProperties).Distinct().ToArray();
            return result;
        }

        /// <summary>
        ///     Determines if a type is numeric.  Nullable numeric types are considered numeric.
        /// </summary>
        /// <remarks>
        ///     Boolean is not considered numeric.
        /// </remarks>
        public static bool IsNumericType(this Type type)
        {
            if (type == null)
                return false;

            var effectiveType = type.GetEffectiveType();

            if (effectiveType.IsEnum())
                return false;

            return NumericTypes.Contains(effectiveType);
        }

        /// <summary>
        ///     Get all classes in a given assembly set.
        /// </summary>
        /// <param name="assemblies">The array of assemblies to look in for classes</param>
        /// <returns>Type list</returns>
        public static IEnumerable<Type> GetClasses(IEnumerable<Assembly> assemblies)
        {
            if (assemblies is null)
                return Array.Empty<Type>();

            return assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type?.GetTypeInfo().IsClass == true);
        }
    }
}
