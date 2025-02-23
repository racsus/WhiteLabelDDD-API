using System;
using System.Linq.Expressions;

namespace WhiteLabel.Domain.Generic
{
    public class SpecificationBase<T>(Expression<Func<T, bool>> specExpression) : ISpecification<T>
    {
        private Func<T, bool> compiledExpression;

        private Func<T, bool> CompiledExpression => compiledExpression ??= SpecExpression.Compile();

        public static ISpecification<T> False { get; } = new SpecificationBase<T>(x => false);
        public static ISpecification<T> True { get; } = new SpecificationBase<T>(x => true);
        public Expression<Func<T, bool>> SpecExpression { get; } = specExpression;

        public bool SatisfiedBy(T obj)
        {
            return CompiledExpression(obj);
        }
    }
}
