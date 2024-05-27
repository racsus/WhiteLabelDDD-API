using System.Linq;
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Domain.Pagination
{
    /// <summary>
    /// Class to realize queries with paged value
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public class EntityPagedValueQuery<TEntity> : PagedValueQuery<TEntity, TEntity>
        where TEntity : class
    {
        /// <summary>
        ///  Creates an instance of <see cref="EntityPagedValueQuery"/>
        /// </summary>
        /// <param name="specification">ISpecification</param>
        /// <param name="take">Take</param>
        /// <param name="skip">Skip</param>
        public EntityPagedValueQuery(ISpecification<TEntity> specification, int? take, int? skip)
            : base(specification, take, skip) { }

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
        protected override SortDescriptor GetDefaultSort()
        {
            return null;
        }
    }
}
