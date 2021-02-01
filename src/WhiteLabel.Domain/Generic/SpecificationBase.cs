using System;
using System.Linq.Expressions;

namespace WhiteLabel.Domain.Generic
{
    public class SpecificationBase<T> : ISpecification<T>
    {
        private Func<T, bool> _compiledExpression;

        private Func<T, bool> CompiledExpression
        {
            get { return _compiledExpression ?? (_compiledExpression = SpecExpression.Compile()); }
        }

        public static ISpecification<T> False { get; } = new SpecificationBase<T>(x => false);
        public static ISpecification<T> True { get; } = new SpecificationBase<T>(x => true);
        public Expression<Func<T, bool>> SpecExpression { get; }

        public bool IsSatisfiedBy(T obj)
        {
            return CompiledExpression(obj);
        }

        public SpecificationBase(Expression<Func<T, bool>> specExpression)
        {
            this.SpecExpression = specExpression;
        }

    }
}
