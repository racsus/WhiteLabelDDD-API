using System.Collections.Generic;
using WhiteLabel.Domain.Pagination;

namespace WhiteLabel.Infrastructure.Data.Pagination
{
    /// <summary>
    /// Multiple query result interface
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IMultipleQueryResult<T> : IQueryResult<T>
        where T : class
    {
        /// <summary>
        /// Data resulting form the query in <typeparamref name="T"/>
        /// </summary>
        IEnumerable<T> Result { get; }
    }
}
