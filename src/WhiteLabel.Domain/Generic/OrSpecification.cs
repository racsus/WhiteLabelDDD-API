using System;
using System.Linq;
using System.Linq.Expressions;

namespace WhiteLabel.Domain.Generic
{
    /// <summary>
    ///     http://devlicio.us/blogs/jeff_perrin/archive/2006/12/13/the-specification-pattern.aspx
    /// </summary>
    public class OrSpecification<T> : CompositeSpecification<T>
    {
        public OrSpecification(ISpecification<T> leftSide, ISpecification<T> rightSide)
            : base(leftSide, rightSide)
        {
        }

        public override Expression<Func<T, bool>> SpecExpression => this.LeftSide.SpecExpression.OrElse(this.RightSide.SpecExpression);

        
    }
}