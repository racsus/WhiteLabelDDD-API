using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Pagination
{
    /// <summary>
    /// Query interface
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    public interface IQuery<TEntity, TResult>
        where TEntity : class
        where TResult : IQueryResult
    {
        /// <summary>
        /// Given a queryable and a queryableEvaluator realizes the query
        /// </summary>
        /// <param name="queryable">IQueryable</param>
        /// <param name="evaluator">IQueryableEvaluator</param>
        /// <returns>Data resulting from the query</returns>
        TResult Run(IQueryable<TEntity> queryable, IQueryableEvaluator evaluator);

        /// <summary>
        /// Given a queryable and a queryableEvaluator realizes the query asynchronously
        /// </summary>
        /// <param name="queryable">IQueryable</param>
        /// <param name="evaluator">IQueryableEvaluator</param>
        /// <param name="cancellationToken">CancelationToken</param>
        /// <returns>Data resulting from the query</returns>
        Task<TResult> RunAsync(
            IQueryable<TEntity> queryable,
            IQueryableEvaluator evaluator,
            CancellationToken cancellationToken = default
        );
    }
}
