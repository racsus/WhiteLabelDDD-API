using System;
using System.Linq.Expressions;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Infrastructure.Data.Extensions;

namespace WhiteLabel.Infrastructure.Data.Pagination
{
    /// <summary>
    ///     http://devlicio.us/blogs/jeff_perrin/archive/2006/12/13/the-specification-pattern.aspx
    /// </summary>
    public class OrSpecification<T> : ComposeSpecification<T>
    {
        public OrSpecification(ISpecification<T> leftSide, ISpecification<T> rightSide)
            : base(leftSide, rightSide) { }

        public override Expression<Func<T, bool>> SpecExpression =>
            Left.SpecExpression.OrElse(Right.SpecExpression);
    }
}
