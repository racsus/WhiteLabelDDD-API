using System.Collections.Generic;

namespace WhiteLabel.Domain.Pagination
{
    /// <summary>
    /// Paged query result interface
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IPagedQueryResult<T> : IQueryResult<T>
        where T : class
    {
        /// <summary>
        /// Take
        /// </summary>
        int? Take { get; }
        /// <summary>
        /// Skip
        /// </summary>
        int? Skip { get; }
        /// <summary>
        /// Total results
        /// </summary>
        int Total { get; }
        /// <summary>
        /// Data resulting form the query in <typeparamref name="T"/>
        /// </summary>
        IEnumerable<T> Result { get; }
    }
}
