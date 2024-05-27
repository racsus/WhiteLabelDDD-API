// ReSharper disable once CheckNamespace
using System.Linq;
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Domain.Extensions
{
    public static class QueryableSpecificationExtensions
    {
        public static IQueryable<T> Where<T>(
            this IQueryable<T> query,
            ISpecification<T> specification
        )
        {
            return query.Where(specification.SpecExpression);
        }
    }
}
