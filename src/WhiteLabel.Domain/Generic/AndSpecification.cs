using System;
using System.Linq;
using System.Linq.Expressions;

namespace WhiteLabel.Domain.Generic
{
    
    public class AndSpecification<T> : CompositeSpecification<T>
    {
        public AndSpecification(ISpecification<T> leftSide, ISpecification<T> rightSide)
            : base(leftSide, rightSide)
        {
        }

        public override Expression<Func<T, bool>> SpecExpression => this.LeftSide.SpecExpression.AndAlso(this.RightSide.SpecExpression);


        
    }
}