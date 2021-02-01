using System.Threading.Tasks;
using WhiteLabel.Application.Interfaces.Generic;
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Infrastructure.Data.Repositories
{
    public class GenericUnitOfWork: IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public GenericUnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void BeginTransaction()
        {
            _dbContext.Database.BeginTransaction();
        }

        public void Commit()
        {
            if (_dbContext.Database.CurrentTransaction != null)
            {
                _dbContext.Database.CommitTransaction();
            }            
            _dbContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            if (_dbContext.Database.CurrentTransaction != null)
            {
                _dbContext.Database.CommitTransaction();
            }
            await _dbContext.SaveChangesAsync();
        }

        public void Rollback()
        {
            _dbContext.Database.RollbackTransaction();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
