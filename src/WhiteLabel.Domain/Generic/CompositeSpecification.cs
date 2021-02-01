using System;
using System.Linq;
using System.Linq.Expressions;

namespace WhiteLabel.Domain.Generic
{
   
    public abstract class CompositeSpecification<T> : ISpecification<T>
    {
        protected readonly ISpecification<T> LeftSide;

        protected readonly ISpecification<T> RightSide;

        protected CompositeSpecification(ISpecification<T> leftSide, ISpecification<T> rightSide)
        {
            this.LeftSide = leftSide;
            this.RightSide = rightSide;
        }

        public abstract Expression<Func<T, bool>> SpecExpression { get; }

        public bool IsSatisfiedBy(T obj)
        {
            throw new NotImplementedException();
        }
    }
}