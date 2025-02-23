using System.Linq;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Pagination;

namespace WhiteLabel.Infrastructure.Data.Pagination
{
    /// <summary>
    /// Class to build pagination in queries
    /// </summary>
    /// <typeparam name="TEntity">Entity type (generic)</typeparam>
    public class EntityPagedValueQuery<TEntity>(
        ISpecification<TEntity> specification,
        int? take,
        int? skip
    ) : PagedValueQuery<TEntity, TEntity>(specification, take, skip)
        where TEntity : class
    {
        /// <summary>
        /// Materializes the query and returns data in <typeparamref name="TEntity"/>
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns>Data resulting from the query</returns>
        protected override IQueryable<TEntity> Materialize(IQueryable<TEntity> queryable)
        {
            return queryable;
        }

        /// <summary>
        /// Returns the default sort descriptor
        /// </summary>
        /// <returns>SortDescriptor</returns>
        protected override SortOption GetDefaultSort()
        {
            return null;
        }
    }
}
