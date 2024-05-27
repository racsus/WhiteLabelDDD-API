using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Pagination;
using WhiteLabel.Infrastructure.Data.Extensions;

namespace WhiteLabel.Infrastructure.Data.Pagination
{
    /// <summary>
    /// Class to realize queries with paged value
    /// </summary>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    /// <typeparam name="TResult">Result Type</typeparam>
    public abstract class PagedValueQuery<TEntity, TResult>
        : Query<TEntity, IPagedQueryResult<TResult>>,
            IPagedValueQuery<TEntity, TResult>
        where TEntity : class
        where TResult : class
    {
        /// <summary>
        /// Counter
        /// </summary>
        private int count;

        /// <summary>
        /// Creates an instance of <see>
        ///     <cref>PagedValueQuery</cref>
        /// </see>
        /// </summary>
        /// <param name="specification">ISpecification</param>
        /// <param name="take">Take</param>
        /// <param name="skip">Skip</param>
        protected PagedValueQuery(ISpecification<TEntity> specification, int? take, int? skip)
            : base(specification)
        {
            Skip = skip;
            Take = take;
        }

        /// <summary>
        /// Sort descriptor
        /// </summary>
        public IEnumerable<SortOption> Sorts { get; init; } = new SortOption[] { };

        /// <summary>
        /// Skip
        /// </summary>
        public int? Skip { get; }

        /// <summary>
        /// Take
        /// </summary>
        public int? Take { get; }

        /// <summary>
        /// Given a Queryable and a QueryableEvaluator realizes the query
        /// </summary>
        /// <param name="queryable">IQueryable</param>
        /// <param name="evaluator">IQueryableEvaluator</param>
        /// <returns>Data resulting from the query</returns>
        public override IPagedQueryResult<TResult> Run(
            IQueryable<TEntity> queryable,
            IQueryableEvaluator evaluator
        )
        {
            queryable = RunQuery(queryable);
            count = evaluator.Count(queryable);
            queryable = Sort(queryable);
            queryable = Paginate(queryable);
            var result = GenerateResult(queryable, evaluator);
            return result;
        }

        /// <summary>
        /// Given a Queryable and a QueryableEvaluator realizes the query asynchronously
        /// </summary>
        /// <param name="queryable">IQueryable</param>
        /// <param name="evaluator">IQueryableEvaluator</param>
        /// <param name="includes"></param>
        /// <param name="cancellationToken">CancelationToken</param>
        /// <returns>Data resulting from the query</returns>
        public override async Task<IPagedQueryResult<TResult>> RunAsync(
            IQueryable<TEntity> queryable,
            IQueryableEvaluator evaluator,
            string[] includes,
            CancellationToken cancellationToken = default
        )
        {
            queryable = RunQuery(queryable);
            count = await evaluator.CountAsync(queryable, cancellationToken);
            queryable = Sort(queryable);
            queryable = Paginate(queryable);
            if (includes != null)
                foreach (var include in includes)
                    queryable = queryable.Include(include);
            var result = await GenerateResultAsync(queryable, evaluator, cancellationToken);
            return result;
        }

        /// <summary>
        /// Sorts the query by the given sorts or by default
        /// </summary>
        /// <param name="queryable">IQueryable</param>
        /// <returns>Data sorted</returns>
        protected virtual IQueryable<TEntity> Sort(IQueryable<TEntity> queryable)
        {
            var effectiveSort = Sorts.ToArray();

            if (!effectiveSort.Any())
            {
                var defaultSort = GetDefaultSort();
                if (defaultSort != null)
                    effectiveSort.Add(defaultSort);
            }

            return queryable.Sort(effectiveSort);
        }

        /// <summary>
        /// Paginates the data resulting from the query
        /// </summary>
        /// <param name="queryable">IQueryable</param>
        /// <returns>Paginated data</returns>
        protected virtual IQueryable<TEntity> Paginate(IQueryable<TEntity> queryable)
        {
            if (Skip.HasValue)
                queryable = queryable.Skip(Skip.Value);

            if (Take.HasValue)
                queryable = queryable.Take(Take.Value);

            return queryable;
        }

        /// <summary>
        /// Generates the query and returns data in <typeparamref name="TResult"/>
        /// </summary>
        /// <param name="queryable">IQueryable</param>
        /// <param name="evaluator">IQueryableEvaluates</param>
        /// <returns>Data resulting from the query</returns>
        protected override IPagedQueryResult<TResult> GenerateResult(
            IQueryable<TEntity> queryable,
            IQueryableEvaluator evaluator
        )
        {
            var materialized = Materialize(queryable);
            var values = evaluator.ToArray(materialized);
            return new PagedQueryResult<TResult>(values, Take, Skip, count);
        }

        /// <summary>
        /// Generates the query asynchronously and returns data in <typeparamref name="TResult"/>
        /// </summary>
        /// <param name="queryable">IQueryable</param>
        /// <param name="evaluator">QueryableEvaluator</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Data resulting from the query</returns>
        protected override async Task<IPagedQueryResult<TResult>> GenerateResultAsync(
            IQueryable<TEntity> queryable,
            IQueryableEvaluator evaluator,
            CancellationToken cancellationToken = default
        )
        {
            var materialized = Materialize(queryable);
            var values = await evaluator.ToArrayAsync(materialized, cancellationToken);
            return new PagedQueryResult<TResult>(values, Take, Skip, count);
        }

        /// <summary>
        ///  Abstract method to be implemented in the inheriting classes that materializes the query and returns data in <typeparamref name="TResult"/>
        /// </summary>
        /// <param name="queryable">IQueryable</param>
        /// <returns>Data resulting from the query</returns>
        protected abstract IQueryable<TResult> Materialize(IQueryable<TEntity> queryable);

        /// <summary>
        /// Abstract method to be implemented in the inheriting classes that returns the default sort descriptor
        /// </summary>
        /// <returns>Default SortDescriptor</returns>
        protected abstract SortOption GetDefaultSort();
    }
}
