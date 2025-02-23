using System;
using System.Linq.Expressions;
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Infrastructure.Data.Pagination
{
    public abstract class ComposeSpecification<T>(ISpecification<T> left, ISpecification<T> right)
        : ISpecification<T>
    {
        protected readonly ISpecification<T> Left = left;

        protected readonly ISpecification<T> Right = right;

        public abstract Expression<Func<T, bool>> SpecExpression { get; }

        public bool SatisfiedBy(T obj)
        {
            throw new NotImplementedException();
        }
    }
}
