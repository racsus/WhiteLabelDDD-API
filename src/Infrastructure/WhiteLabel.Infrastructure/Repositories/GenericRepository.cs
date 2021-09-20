using WhiteLabel.Infrastructure.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WhiteLabel.Application.Interfaces.Generic;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Pagination;

namespace WhiteLabel.Infrastructure.Data.Repositories
{
    public class GenericRepository<TId> : IGenericRepository<TId>
    {
        private readonly AppDbContext _dbContext;
        protected IQueryableEvaluator Evaluator { get; }
        protected ISpecificationBuilder SpecificationBuilder { get; }

        public GenericRepository(AppDbContext dbContext,
            IQueryableEvaluator evaluator,
            ISpecificationBuilder specificationBuilder)
        {
            _dbContext = dbContext;
            this.Evaluator = evaluator;
            this.SpecificationBuilder = specificationBuilder;
        }

        public T FindById<T>(TId id) where T : BaseEntity<TId>
        {
            return _dbContext.Set<T>().SingleOrDefault(e => e.Id.Equals(id));
        }

        public T FindById<T>(TId id, string[] includes) where T : BaseEntity<TId>
        {
            var query = _dbContext.Set<T>().AsQueryable();

            foreach (string include in includes)
            {
                query = query.Include(include);
            }

            return query.SingleOrDefault(e => e.Id.Equals(id));
        }

        public async Task<T> FindByIdAsync<T>(TId id, CancellationToken cancellationToken = default) where T : BaseEntity<TId>
        {
            return await _dbContext.Set<T>().SingleOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        }

        public async Task<T> FindByIdAsync<T>(TId id, string[] includes, CancellationToken cancellationToken = default) where T : BaseEntity<TId>
        {
            var query = _dbContext.Set<T>().AsQueryable();

            foreach (string include in includes)
            {
                query = query.Include(include);
            }

            return await Task.FromResult(query.SingleOrDefault(e => e.Id.Equals(id)));
        }

        public IEnumerable<T> Find<T>(ISpecification<T> spec) where T : BaseEntity<TId>
        {
            return _dbContext.Set<T>().Where(spec.IsSatisfiedBy).ToList();
        }

        public IEnumerable<T> Find<T>(ISpecification<T> spec, string[] includes) where T : BaseEntity<TId>
        {
            var query = _dbContext.Set<T>().Where(spec.IsSatisfiedBy).AsQueryable();

            foreach (string include in includes)
            {
                query = query.Include(include);
            }

            return query.ToList();
        }

        public async Task<IEnumerable<T>> FindAsync<T>(ISpecification<T> spec, CancellationToken cancellationToken = default) where T : BaseEntity<TId>
        {
            var query = _dbContext.Set<T>().Where(spec.IsSatisfiedBy).AsQueryable();
            return await Task.FromResult(query.ToList());
        }

        public async Task<IEnumerable<T>> FindAsync<T>(ISpecification<T> spec, string[] includes, CancellationToken cancellationToken = default) where T : BaseEntity<TId>
        {
            var query = _dbContext.Set<T>().AsQueryable();

            foreach (string include in includes)
            {
                query = query.Include(include);
            }

            return await Task.FromResult(query.Where(spec.IsSatisfiedBy).ToList());
        }

        public T FindOne<T>(ISpecification<T> spec) where T : BaseEntity<TId>
        {
            return _dbContext.Set<T>().Where(spec.IsSatisfiedBy).FirstOrDefault();
        }

        public async Task<T> FindOneAsync<T>(ISpecification<T> spec, CancellationToken cancellationToken = default) where T : BaseEntity<TId>
        {
            var query = _dbContext.Set<T>().Where(spec.IsSatisfiedBy).AsQueryable();
            return await Task.FromResult(query.FirstOrDefault());
        }

        public T FindOne<T>(ISpecification<T> spec, string[] includes) where T : BaseEntity<TId>
        {
            var query = _dbContext.Set<T>().AsQueryable();

            foreach (string include in includes)
            {
                query = query.Include(include);
            }

            return query.Where(spec.IsSatisfiedBy).FirstOrDefault();
        }

        public async Task<T> FindOneAsync<T>(ISpecification<T> spec, string[] includes, CancellationToken cancellationToken = default) where T : BaseEntity<TId>
        {
            var query = _dbContext.Set<T>().AsQueryable();
            foreach (string include in includes)
            {
                query = query.Include(include);
            }

            var res = await Task.FromResult(query.Where(spec.IsSatisfiedBy).FirstOrDefault());
            return res;
        }

        public T Add<T>(T entity) where T : BaseEntity<TId>
        {
            _dbContext.Set<T>().Add(entity);
            return entity;
        }

        public async Task<T> AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : BaseEntity<TId>
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public void Delete<T>(T entity) where T : BaseEntity<TId>
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public void Update<T>(T entity) where T : BaseEntity<TId>
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public IEnumerable<T> FindAll<T>() where T : BaseEntity<TId>
        {
            return _dbContext.Set<T>().ToList();
        }

        public async Task<IEnumerable<T>> FindAllAsync<T>(CancellationToken cancellationToken = default) where T : BaseEntity<TId>
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public IEnumerable<T> FindAll<T>(string[] includes) where T : BaseEntity<TId>
        {
            return _dbContext.Set<T>().ToList();
        }

        public int Count<T>() where T : BaseEntity<TId>
        {
            return _dbContext.Set<T>().Count();
        }

        public async Task<int> CountAsync<T>(CancellationToken cancellationToken = default) where T : BaseEntity<TId>
        {
            return await _dbContext.Set<T>().CountAsync();
        }

        public int Count<T>(ISpecification<T> spec) where T : BaseEntity<TId>
        {
            return _dbContext.Set<T>().Where(spec.IsSatisfiedBy).Count();
        }

        public async Task<int> CountAsync<T>(ISpecification<T> spec, CancellationToken cancellationToken = default) where T : BaseEntity<TId>
        {
            var query = _dbContext.Set<T>().Where(spec.IsSatisfiedBy).AsQueryable();
            return await Task.FromResult(query.Count());
        }

        public async Task<IEnumerable<T>> FindAllAsync<T>(string[] includes, CancellationToken cancellationToken = default) where T : BaseEntity<TId>
        {
            var query = _dbContext.Set<T>().AsQueryable();
            foreach (string include in includes)
            {
                query = query.Include(include);
            }
            return await Task.FromResult(query.ToList());
        }

        public IPagedQueryResult<T> FindPaged<T>(IPageOption pageOptions, string[] includes) where T : BaseEntity<TId>
        {
            if (pageOptions == null)
            {
                throw new ArgumentNullException(nameof(pageOptions));
            }
            var spec = this.SpecificationBuilder.Create<T>(pageOptions.Filters);
            var entityQuery = new EntityPagedValueQuery<T>(spec, pageOptions.Take, pageOptions.Skip)
            {
                Sorts = pageOptions.Sorts
            };

            var entityQueryResult = entityQuery.Run(_dbContext.Set<T>(), this.Evaluator);
            var modelsQueryResult = new PagedQueryResult<T>(entityQueryResult.Result,
                entityQueryResult.Take, entityQueryResult.Skip, entityQueryResult.Total);
            return modelsQueryResult;
        }

        public async Task<IPagedQueryResult<T>> FindPagedAsync<T>(IPageOption pageOptions, string[] includes, CancellationToken cancellationToken = default) where T : BaseEntity<TId>
        {
            if (pageOptions == null)
            {
                throw new ArgumentNullException(nameof(pageOptions));
            }
            var spec = this.SpecificationBuilder.Create<T>(pageOptions.Filters);
            var entityQuery = new EntityPagedValueQuery<T>(spec, pageOptions.Take, pageOptions.Skip)
            {
                Sorts = pageOptions.Sorts
            };

            var entityQueryResult = await entityQuery.RunAsync(_dbContext.Set<T>(), this.Evaluator, includes, cancellationToken);
            var modelsQueryResult = new PagedQueryResult<T>(entityQueryResult.Result,
                entityQueryResult.Take, entityQueryResult.Skip, entityQueryResult.Total);
            return modelsQueryResult;
        }

        public async Task<IEnumerable<string>> FindGroup<T>(string fieldToGroup, string[] includes, CancellationToken cancellationToken = default) where T : BaseEntity<TId>
        {
            var query = _dbContext.Set<T>().AsQueryable();
            foreach (string include in includes)
            {
                query = query.Include(include);
            }

            return await query.GroupBy(LinQHelper<T>.GetExpressionByName(fieldToGroup)).Select(x => x.Key).ToListAsync();
        }

    }
}
