using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Threading.Tasks;
using WhiteLabel.Domain.Pagination;
using WhiteLabel.Infrastructure.Data;
using WhiteLabel.Infrastructure.Data.Repositories;
using WhiteLabel.Infrastructure.Events;

namespace WhiteLabel.UnitTest.Repository
{
    public abstract class BaseAppDbContextTest<TId>
    {
        protected AppDbContext _dbContext;
        protected GenericRepository<TId> Repository;

        public BaseAppDbContextTest()
        {
            this.Repository = GetRepository();
        }

        protected static DbContextOptions<AppDbContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseInMemoryDatabase("cleanarchitecture")
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        protected GenericRepository<TId> GetRepository()
        {
            var options = CreateNewContextOptions();
            var mockDispatcher = new Mock<IDomainEventDispatcher>();

            SpecificationBuilder specificationBuilder = new SpecificationBuilder(new NullLogger<SpecificationBuilder>());
            EfCoreQueryableEvaluator evaluator = new EfCoreQueryableEvaluator();
            _dbContext = new AppDbContext(options, mockDispatcher.Object);

            return new GenericRepository<TId>(_dbContext, evaluator, specificationBuilder);
        }

        protected void SaveChanges()
        {
            if (_dbContext != null)
            {
                _dbContext.SaveChanges();
            }
        }

        protected async Task SaveChangesAsync()
        {
            if (_dbContext != null)
            {
                await _dbContext.SaveChangesAsync();
            }
        }

        protected void ClearMemory()
        {
            if (_dbContext != null)
            {
                _dbContext.Database.EnsureDeleted();
            }
        }

        protected async Task ClearMemoryAsync()
        {
            if (_dbContext != null)
            {
                await _dbContext.Database.EnsureDeletedAsync();
            }
        }

    }
}
