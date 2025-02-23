using System.Threading.Tasks;
using WhiteLabel.Application.Interfaces.Generic;

namespace WhiteLabel.Infrastructure.Data.Repositories
{
    public class GenericUnitOfWork(AppDbContext dbContext) : IUnitOfWork
    {
        public void BeginTransaction()
        {
            dbContext.Database.BeginTransaction();
        }

        public void Commit()
        {
            if (dbContext.Database.CurrentTransaction != null)
                dbContext.Database.CommitTransaction();
            dbContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            if (dbContext.Database.CurrentTransaction != null)
                await dbContext.Database.CommitTransactionAsync();
            await dbContext.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public void Rollback()
        {
            dbContext.Database.RollbackTransaction();
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}
