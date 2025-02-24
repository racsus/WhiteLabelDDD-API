﻿using System;
using System.Linq.Expressions;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Infrastructure.Data.Extensions;

namespace WhiteLabel.Infrastructure.Data.Pagination
{
    public class AndSpecification<T>(ISpecification<T> leftSide, ISpecification<T> rightSide)
        : ComposeSpecification<T>(leftSide, rightSide)
    {
        public override Expression<Func<T, bool>> SpecExpression =>
            Left.SpecExpression.AndAlso(Right.SpecExpression);
    }
}
