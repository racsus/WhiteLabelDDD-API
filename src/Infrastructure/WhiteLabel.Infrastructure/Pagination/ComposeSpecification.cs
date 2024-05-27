using System;
using System.Linq.Expressions;
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Infrastructure.Data.Pagination
{
    public abstract class ComposeSpecification<T> : ISpecification<T>
    {
        protected readonly ISpecification<T> Left;

        protected readonly ISpecification<T> Right;

        protected ComposeSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            Left = left;
            Right = right;
        }

        public abstract Expression<Func<T, bool>> SpecExpression { get; }

        public bool IsSatisfiedBy(T obj)
        {
            throw new NotImplementedException();
        }
    }
}
