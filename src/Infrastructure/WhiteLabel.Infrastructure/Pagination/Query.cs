using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Pagination;
using WhiteLabel.Infrastructure.Data.Extensions;

namespace WhiteLabel.Infrastructure.Data.Pagination
{
    /// <summary>
    /// Abstract class Query
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TResult">Result type</typeparam>
    public abstract class Query<TEntity, TResult> : IQuery<TEntity, TResult>
        where TEntity : class
        where TResult : class, IQueryResult
    {
        /// <summary>
        /// Constructor for <see>
        ///     <cref>Query</cref>
        /// </see>
        /// to be called from the inheriting classes
        /// </summary>
        /// <param name="specification">Specification to realize the query</param>
        protected Query(ISpecification<TEntity> specification)
        {
            Specification = specification;
        }

        /// <summary>
        /// Specification to realize the query
        /// </summary>
        protected ISpecification<TEntity> Specification { get; }

        /// <summary>
        /// Given a queryable and a queryableEvaluator realizes the query
        /// </summary>
        /// <param name="queryable">IQueriable</param>
        /// <param name="evaluator">IQueryableEvaluator</param>
        /// <returns>Data resulting from the query</returns>
        public virtual TResult Run(IQueryable<TEntity> queryable, IQueryableEvaluator evaluator)
        {
            var processedQuery = RunQuery(queryable);
            var result = GenerateResult(processedQuery, evaluator);
            return result;
        }

        /// <summary>
        /// Given a queryable and a queryableEvaluator realizes the query asynchronously
        /// </summary>
        /// <param name="queryable">IQueryable</param>
        /// <param name="evaluator">IQueryableEvaluator</param>
        /// <param name="includes"></param>
        /// <param name="cancellationToken">CancelationsToken</param>
        /// <returns>Data resulting from the query</returns>
        public virtual async Task<TResult> RunAsync(
            IQueryable<TEntity> queryable,
            IQueryableEvaluator evaluator,
            string[] includes,
            CancellationToken cancellationToken = default
        )
        {
            var processedQuery = RunQuery(queryable);
            var result = await GenerateResultAsync(processedQuery, evaluator, cancellationToken);
            return result;
        }

        /// <summary>
        /// Protected method that performs the query
        /// </summary>
        /// <param name="queryable">IQueryable</param>
        /// <returns>IQueryable</returns>
        protected virtual IQueryable<TEntity> RunQuery(IQueryable<TEntity> queryable)
        {
            return queryable.Where(Specification);
        }

        /// <summary>
        /// Abstract method to be implemented in the inheriting classes that generates the query result depending on <typeparamref name="TResult"/>
        /// </summary>
        /// <param name="queryable">IQueryable</param>
        /// <param name="evaluator">IQueryableEvaluator</param>
        /// <returns>Data resulting from the query</returns>
        protected abstract TResult GenerateResult(
            IQueryable<TEntity> queryable,
            IQueryableEvaluator evaluator
        );

        /// <summary>
        /// Abstract method to be implemented in the inheriting classes that generates asynchronously the query result <typeparamref name="TResult"/>
        /// </summary>
        /// <param name="queryable">IQueryable</param>
        /// <param name="evaluator">IQueryableEvaluator</param>
        /// <param name="cancellationToken">CancelationToken</param>
        /// <returns>Data resulting from the query</returns>
        protected abstract Task<TResult> GenerateResultAsync(
            IQueryable<TEntity> queryable,
            IQueryableEvaluator evaluator,
            CancellationToken cancellationToken = default
        );
    }
}
