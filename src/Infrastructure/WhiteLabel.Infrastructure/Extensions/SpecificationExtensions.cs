using WhiteLabel.Domain.Generic;
using WhiteLabel.Infrastructure.Data.Pagination;

namespace WhiteLabel.Infrastructure.Data.Extensions
{
    public static class SpecificationExtensions
    {
        public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            return new AndSpecification<T>(left, right);
        }

        public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            return new OrSpecification<T>(left, right);
        }
    }
}
