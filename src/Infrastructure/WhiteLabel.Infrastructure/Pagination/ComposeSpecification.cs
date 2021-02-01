using System;
using System.Linq;
using System.Linq.Expressions;

namespace WhiteLabel.Domain.Generic
{
   
    public abstract class ComposeSpecification<T> : ISpecification<T>
    {
        protected readonly ISpecification<T> Left;

        protected readonly ISpecification<T> Right;

        protected ComposeSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            this.Left = left;
            this.Right = right;
        }

        public abstract Expression<Func<T, bool>> SpecExpression { get; }

        public bool IsSatisfiedBy(T obj)
        {
            throw new NotImplementedException();
        }
    }
}