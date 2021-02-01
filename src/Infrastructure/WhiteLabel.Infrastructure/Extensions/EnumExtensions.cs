using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WhiteLabel.Domain.Extensions
{
    public static class EnumExtensions
    {
        private static DisplayAttribute GetDisplayAttribute(object value)
        {
            Type type = value.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException(string.Format("Type {0} is not an enum", type));
            }

            // Get the enum field.
            var field = type.GetField(value.ToString());
            return field == null ? null : field.GetCustomAttribute<DisplayAttribute>();
        }
    }
}
