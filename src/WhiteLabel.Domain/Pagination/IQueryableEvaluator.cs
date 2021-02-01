using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WhiteLabel.Domain.Pagination
{
    /// <summary>
    /// Queryable evaluator interface
    /// </summary>
    public interface IQueryableEvaluator
    {
        /// <summary>
        /// Counts the number of items
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="source">IQueryable</param>
        /// <returns>Number of items</returns>
        int Count<T>(IQueryable<T> source) where T : class;

        /// <summary>
        /// Counts asynchronously the number of items
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="source">IQueryable</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Number of items</returns>
        Task<int> CountAsync<T>(IQueryable<T> source,CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Returns if exists any item in the IQueryable
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="source">IQueryable</param>
        /// <returns>True if there are elements or false if not</returns>
        bool Any<T>(IQueryable<T> source) where T : class;

        /// <summary>
        /// Returns if exists any item in the IQueryable (asynchronous)
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="source">IQueriable</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>True if there are elements or false if not</returns>
        Task<bool> AnyAsync<T>(IQueryable<T> source,CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Returns the first or default item in the IQueryable
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="source">IQueryable</param>
        /// <returns>The first or default item</returns>
        T FirstOrDefault<T>(IQueryable<T> source) where T : class;

        /// <summary>
        /// Returns the first or default item in the IQueryable (asynchronous)
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="source">IQueryable</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>The first or default item</returns>
        Task<T> FirstOrDefaultAsync<T>(IQueryable<T> source,CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Returns a single or default item in the IQueryable
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="source">IQueryable</param>
        /// <returns>A single or default element</returns>
        T SingleOrDefault<T>(IQueryable<T> source) where T : class;

        /// <summary>
        /// Returns a single or default item in the IQueryable
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="source">IQueryable</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>A single or default element</returns>
        Task<T> SingleOrDefaultAsync<T>(IQueryable<T> source,CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Copy all the elements to an array
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="source">IQueryable</param>
        /// <returns>All the elements in the IQueryable</returns>
        T[] ToArray<T>(IQueryable<T> source) where T : class;

        /// <summary>
        /// Copy all the elements to an array (asynchronous)
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="source">IQueryable</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>All the elements in the IQueryable</returns>
        Task<T[]> ToArrayAsync<T>(IQueryable<T> source,CancellationToken cancellationToken = default) where T : class;
    }
}
