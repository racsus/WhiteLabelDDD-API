using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Threading.Tasks;
using WhiteLabel.Infrastructure.Data;
using WhiteLabel.Infrastructure.Data.Pagination;
using WhiteLabel.Infrastructure.Data.Repositories;
using WhiteLabel.Infrastructure.Events;

namespace WhiteLabel.UnitTest.Repository
{
    public abstract class BaseAppDbContextTest<TId>
    {
        protected AppDbContext DbContext;
        protected GenericRepository<TId> Repository;

        public BaseAppDbContextTest()
        {
            Repository = GetRepository();
        }

        private static DbContextOptions<AppDbContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder
                .UseInMemoryDatabase("WhiteLabelMemoryDatabase")
                .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        private GenericRepository<TId> GetRepository()
        {
            var options = CreateNewContextOptions();
            var mockDispatcher = new Mock<IDomainEventDispatcher>();

            var specificationBuilder = new SpecificationBuilder(
                new NullLogger<SpecificationBuilder>()
            );
            var evaluator = new EfCoreQueryableEvaluator();
            DbContext = new AppDbContext(options, mockDispatcher.Object);

            return new GenericRepository<TId>(DbContext, evaluator, specificationBuilder);
        }

        protected void SaveChanges()
        {
            if (DbContext != null)
                DbContext.SaveChanges();
        }

        protected async Task SaveChangesAsync()
        {
            if (DbContext != null)
                await DbContext.SaveChangesAsync();
        }

        protected void ClearMemory()
        {
            if (DbContext != null)
                DbContext.Database.EnsureDeleted();
        }

        protected async Task ClearMemoryAsync()
        {
            if (DbContext != null)
                await DbContext.Database.EnsureDeletedAsync();
        }
    }
}
