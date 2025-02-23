using System;
using System.Linq.Expressions;

namespace WhiteLabel.Domain.Generic
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> SpecExpression { get; }
        bool SatisfiedBy(T obj);
    }
}
