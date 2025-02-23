using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhiteLabel.Infrastructure.Data.Pagination;

namespace WhiteLabel.Infrastructure.Data
{
    public class EfCoreQueryableEvaluator : IQueryableEvaluator
    {
        public int Count<T>(IQueryable<T> source)
            where T : class
        {
            return source.Count();
        }

        public Task<int> CountAsync<T>(
            IQueryable<T> source,
            CancellationToken cancellationToken = default
        )
            where T : class
        {
            return source.CountAsync(cancellationToken);
        }

        public bool Any<T>(IQueryable<T> source)
            where T : class
        {
            return source.Any();
        }

        public Task<bool> AnyAsync<T>(
            IQueryable<T> source,
            CancellationToken cancellationToken = default
        )
            where T : class
        {
            return source.AnyAsync(cancellationToken);
        }

        public T FirstOrDefault<T>(IQueryable<T> source)
            where T : class
        {
            return source.FirstOrDefault();
        }

        public Task<T> FirstOrDefaultAsync<T>(
            IQueryable<T> source,
            CancellationToken cancellationToken = default
        )
            where T : class
        {
            return source.FirstOrDefaultAsync(cancellationToken);
        }

        public T SingleOrDefault<T>(IQueryable<T> source)
            where T : class
        {
            return source.SingleOrDefault();
        }

        public Task<T> SingleOrDefaultAsync<T>(
            IQueryable<T> source,
            CancellationToken cancellationToken = default
        )
            where T : class
        {
            return source.SingleOrDefaultAsync(cancellationToken);
        }

        public T[] ToArray<T>(IQueryable<T> source)
            where T : class
        {
            return source.ToArray();
        }

        public Task<T[]> ToArrayAsync<T>(
            IQueryable<T> source,
            CancellationToken cancellationToken = default
        )
            where T : class
        {
            return source.ToArrayAsync(cancellationToken);
        }
    }
}
