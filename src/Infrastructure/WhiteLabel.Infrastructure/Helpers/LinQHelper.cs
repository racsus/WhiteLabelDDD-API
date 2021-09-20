using System;
using System.Linq.Expressions;

namespace WhiteLabel.Infrastructure.Data.Helpers
{
    public static class LinQHelper<T>
    {
        public static Expression<Func<T, string>> GetExpressionByName(string property)
        {
            var parameter = Expression.Parameter(typeof(T));
            var body = Expression.Property(parameter, property);
            return Expression.Lambda<Func<T, string>>(body, parameter);
        }

    }
}
