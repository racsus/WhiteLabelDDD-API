using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.Infrastructure.Data
{
    public interface IAppDbContext
    {
        //Users
        DbSet<User> Users { get; set; }

        DatabaseFacade DataBase { get; set; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
