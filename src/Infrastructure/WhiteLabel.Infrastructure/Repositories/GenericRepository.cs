using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhiteLabel.Application.Interfaces.Generic;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Pagination;
using WhiteLabel.Infrastructure.Data.Extensions;
using WhiteLabel.Infrastructure.Data.Pagination;

namespace WhiteLabel.Infrastructure.Data.Repositories
{
    public class GenericRepository<TId>(
        AppDbContext dbContext,
        IQueryableEvaluator evaluator,
        ISpecificationBuilder specificationBuilder
    ) : IGenericRepository<TId>
    {
        private IQueryableEvaluator Evaluator { get; } = evaluator;
        private ISpecificationBuilder SpecificationBuilder { get; } = specificationBuilder;

        public T FindById<T>(TId id)
            where T : BaseEntity<TId>
        {
            return dbContext.Set<T>().SingleOrDefault(e => e.Id.Equals(id));
        }

        public T FindById<T>(TId id, IEnumerable<string> includes)
            where T : BaseEntity<TId>
        {
            var query = dbContext.Set<T>().AsQueryable();

            foreach (var include in includes)
                query = query.Include(include);

            return query.SingleOrDefault(e => e.Id.Equals(id));
        }

        public async Task<T> FindByIdAsync<T>(TId id, CancellationToken cancellationToken = default)
            where T : BaseEntity<TId>
        {
            return await dbContext
                .Set<T>()
                .SingleOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        }

        public async Task<T> FindByIdAsync<T>(
            TId id,
            IEnumerable<string> includes,
            CancellationToken cancellationToken = default
        )
            where T : BaseEntity<TId>
        {
            var query = dbContext.Set<T>().AsQueryable();

            foreach (var include in includes)
                query = query.Include(include);

            return await Task.FromResult(query.SingleOrDefault(e => e.Id.Equals(id)));
        }

        public IEnumerable<T> Find<T>(ISpecification<T> spec)
            where T : BaseEntity<TId>
        {
            return dbContext.Set<T>().Where(spec.SatisfiedBy).ToList();
        }

        public IEnumerable<T> Find<T>(ISpecification<T> spec, IEnumerable<string> includes)
            where T : BaseEntity<TId>
        {
            var query = dbContext.Set<T>().Where(spec.SatisfiedBy).AsQueryable();

            foreach (var include in includes)
                query = query.Include(include);

            return query.ToList();
        }

        public async Task<IEnumerable<T>> FindAsync<T>(
            ISpecification<T> spec,
            CancellationToken cancellationToken = default
        )
            where T : BaseEntity<TId>
        {
            var query = dbContext.Set<T>().Where(spec.SatisfiedBy).AsQueryable();
            return await Task.FromResult(query.ToList());
        }

        public async Task<IEnumerable<T>> FindAsync<T>(
            ISpecification<T> spec,
            string[] includes,
            CancellationToken cancellationToken = default
        )
            where T : BaseEntity<TId>
        {
            var query = dbContext.Set<T>().AsQueryable();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await Task.FromResult(query.AsEnumerable().Where(spec.SatisfiedBy).ToList());
        }

        public async Task<IEnumerable<T>> FindAsync<T>(
            IEnumerable<FilterOption> filters,
            ISpecification<T> spec = null,
            string[] includes = null,
            CancellationToken cancellationToken = default
        )
            where T : BaseEntity<TId>
        {
            var query = dbContext.Set<T>().AsQueryable();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            var additionalSpec = SpecificationBuilder.Create<T>(filters);
            if (spec == null)
                spec = additionalSpec;
            else
                spec = spec.And(additionalSpec);

            return await Task.FromResult(query.Where(spec).ToList());
        }

        public T FindOne<T>(ISpecification<T> spec)
            where T : BaseEntity<TId>
        {
            return dbContext.Set<T>().Where(spec.SatisfiedBy).FirstOrDefault();
        }

        public async Task<T> FindOneAsync<T>(
            ISpecification<T> spec,
            CancellationToken cancellationToken = default
        )
            where T : BaseEntity<TId>
        {
            var query = dbContext.Set<T>().Where(spec.SatisfiedBy).AsQueryable();
            return await Task.FromResult(query.FirstOrDefault());
        }

        public T FindOne<T>(ISpecification<T> spec, IEnumerable<string> includes)
            where T : BaseEntity<TId>
        {
            var query = dbContext.Set<T>().AsQueryable();

            foreach (var include in includes)
                query = query.Include(include);

            return query.AsEnumerable().Where(spec.SatisfiedBy).FirstOrDefault();
        }

        public async Task<T> FindOneAsync<T>(
            ISpecification<T> spec,
            string[] includes,
            CancellationToken cancellationToken = default
        )
            where T : BaseEntity<TId>
        {
            var query = dbContext.Set<T>().AsQueryable();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            var res = await Task.FromResult(query.Where(spec.SatisfiedBy).FirstOrDefault());
            return res;
        }

        public T Add<T>(T entity)
            where T : BaseEntity<TId>
        {
            dbContext.Set<T>().Add(entity);
            return entity;
        }

        public async Task<T> AddAsync<T>(T entity, CancellationToken cancellationToken = default)
            where T : BaseEntity<TId>
        {
            await dbContext.Set<T>().AddAsync(entity, cancellationToken);
            return entity;
        }

        public void Delete<T>(T entity)
            where T : BaseEntity<TId>
        {
            dbContext.Set<T>().Remove(entity);
        }

        public void Update<T>(T entity)
            where T : BaseEntity<TId>
        {
            dbContext.Entry(entity).State = EntityState.Modified;
        }

        public IEnumerable<T> FindAll<T>()
            where T : BaseEntity<TId>
        {
            return dbContext.Set<T>().ToList();
        }

        public async Task<IEnumerable<T>> FindAllAsync<T>(
            CancellationToken cancellationToken = default
        )
            where T : BaseEntity<TId>
        {
            return await dbContext.Set<T>().ToListAsync(cancellationToken: cancellationToken);
        }

        public IEnumerable<T> FindAll<T>(string[] includes)
            where T : BaseEntity<TId>
        {
            return dbContext.Set<T>().ToList();
        }

        public int Count<T>()
            where T : BaseEntity<TId>
        {
            return dbContext.Set<T>().Count();
        }

        public async Task<int> CountAsync<T>(CancellationToken cancellationToken = default)
            where T : BaseEntity<TId>
        {
            return await dbContext.Set<T>().CountAsync(cancellationToken: cancellationToken);
        }

        public int Count<T>(ISpecification<T> spec)
            where T : BaseEntity<TId>
        {
            return dbContext.Set<T>().Where(spec.SatisfiedBy).Count();
        }

        public async Task<int> CountAsync<T>(
            ISpecification<T> spec,
            CancellationToken cancellationToken = default
        )
            where T : BaseEntity<TId>
        {
            var query = dbContext.Set<T>().Where(spec.SatisfiedBy).AsQueryable();
            return await Task.FromResult(query.Count());
        }

        public async Task<IEnumerable<T>> FindAllAsync<T>(
            string[] includes,
            CancellationToken cancellationToken = default
        )
            where T : BaseEntity<TId>
        {
            var query = dbContext.Set<T>().AsQueryable();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);
            return await Task.FromResult(query.ToList());
        }

        public IPagedQueryResult<T> FindPaged<T>(
            IPageOption pageOptions,
            string[] includes,
            ISpecification<T> additionalSpec = null
        )
            where T : BaseEntity<TId>
        {
            if (pageOptions == null)
                throw new ArgumentNullException(nameof(pageOptions));
            var spec = SpecificationBuilder.Create<T>(pageOptions.Filters);
            if (additionalSpec != null)
                spec = spec.And(additionalSpec);
            var entityQuery = new EntityPagedValueQuery<T>(spec, pageOptions.Take, pageOptions.Skip)
            {
                Sorts = pageOptions.Sorts,
            };

            var entityQueryResult = entityQuery.Run(dbContext.Set<T>(), Evaluator);
            var modelsQueryResult = new PagedQueryResult<T>(
                entityQueryResult.Result,
                entityQueryResult.Take,
                entityQueryResult.Skip,
                entityQueryResult.Total
            );
            return modelsQueryResult;
        }

        public async Task<IPagedQueryResult<T>> FindPagedAsync<T>(
            IPageOption pageOptions,
            string[] includes,
            ISpecification<T> additionalSpec = null,
            CancellationToken cancellationToken = default
        )
            where T : BaseEntity<TId>
        {
            if (pageOptions == null)
                throw new ArgumentNullException(nameof(pageOptions));
            var spec = SpecificationBuilder.Create<T>(pageOptions.Filters);
            if (additionalSpec != null)
                spec = spec.And(additionalSpec);
            var entityQuery = new EntityPagedValueQuery<T>(spec, pageOptions.Take, pageOptions.Skip)
            {
                Sorts = pageOptions.Sorts,
            };

            var entityQueryResult = await entityQuery.RunAsync(
                dbContext.Set<T>(),
                Evaluator,
                includes,
                cancellationToken
            );
            var modelsQueryResult = new PagedQueryResult<T>(
                entityQueryResult.Result,
                entityQueryResult.Take,
                entityQueryResult.Skip,
                entityQueryResult.Total
            );
            return modelsQueryResult;
        }

        public async Task<IEnumerable<string>> FindGroupAsync<T>(
            string fieldToGroup,
            string[] includes,
            CancellationToken cancellationToken = default
        )
            where T : BaseEntity<TId>
        {
            var query = dbContext.Set<T>().AsQueryable();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            var lambda = ExpressionExtensions.MakeLambdaSelectorExpression<T>(fieldToGroup);
            var res = await query
                .GroupBy(lambda)
                .Select(x => x.Key)
                .ToListAsync(cancellationToken: cancellationToken);
            res = res.Where(x => !string.IsNullOrEmpty(x)).ToList();
            return res;
        }

        public async Task<IEnumerable<string>> FindGroupAsync<T>(
            string fieldToGroup,
            string[] includes,
            ISpecification<T> spec,
            CancellationToken cancellationToken = default
        )
            where T : BaseEntity<TId>
        {
            var query = dbContext.Set<T>().AsQueryable();
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            var lambda = ExpressionExtensions.MakeLambdaSelectorExpression<T>(fieldToGroup);
            if (spec != null)
            {
                var res = await Task.FromResult(
                    query
                        .AsEnumerable()
                        .Where(spec.SatisfiedBy)
                        .AsQueryable()
                        .GroupBy(lambda)
                        .Select(x => x.Key)
                        .ToList()
                );
                res = res.Where(x => !string.IsNullOrEmpty(x)).ToList();
                return res;
            }
            else
            {
                var res = await Task.FromResult(query.GroupBy(lambda).Select(x => x.Key).ToList());
                res = res.Where(x => !string.IsNullOrEmpty(x)).ToList();
                return res;
            }
        }

        public async Task<IEnumerable<T>> FindBySqlAsync<T>(
            string sql,
            CancellationToken cancellationToken = default
        )
            where T : BaseEntity<TId>
        {
            var query = dbContext.Set<T>().FromSqlRaw(sql).AsQueryable();

            return await Task.FromResult(query.ToList());
        }
    }
}
