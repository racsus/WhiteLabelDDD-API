using System;
using System.Linq;
using System.Linq.Expressions;

namespace WhiteLabel.Domain.Generic
{
    
    public class AndSpecification<T> : ComposeSpecification<T>
    {
        public AndSpecification(ISpecification<T> leftSide, ISpecification<T> rightSide)
            : base(leftSide, rightSide)
        {
        }

        public override Expression<Func<T, bool>> SpecExpression => this.Left.SpecExpression.AndAlso(this.Right.SpecExpression);


        
    }
}