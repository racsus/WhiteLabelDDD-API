using System;
using System.Linq;
using System.Reflection;

namespace WhiteLabel.Infrastructure.Data.Extensions
{
    public static class EnumerableExtensions
    {
        public static readonly MethodInfo ContainsMethod = ExpressionExtensions.GetMethodInfo(
            typeof(Enumerable),
            "Contains",
            2
        );

        public static readonly MethodInfo CastMethod = ExpressionExtensions.GetMethodInfo(
            typeof(Enumerable),
            "Cast",
            1
        );

        public static readonly MethodInfo AllMethod = ExpressionExtensions.GetMethodInfo(
            typeof(Enumerable),
            "All",
            2
        );
        public static readonly MethodInfo AnyMethod = ExpressionExtensions.GetMethodInfo(
            typeof(Enumerable),
            "Any",
            2
        );

        public static readonly MethodInfo ToDateStringMethod = ExpressionExtensions.GetMethodInfo(
            typeof(DateTime),
            "ToString",
            0
        );

        public static readonly MethodInfo ToShortDateStringMethod =
            ExpressionExtensions.GetMethodInfo(typeof(DateTime), "ToShortDateString", 0);

        public static readonly MethodInfo RemoveMethod = ExpressionExtensions.GetMethodInfo(
            typeof(string),
            "Remove",
            2
        );

        public static readonly MethodInfo StringContainsMethod = ExpressionExtensions.GetMethodInfo(
            typeof(string),
            "Contains",
            1
        );
    }
}
