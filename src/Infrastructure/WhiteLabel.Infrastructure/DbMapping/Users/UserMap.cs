using Microsoft.EntityFrameworkCore;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.Infrastructure.Data.DbMapping.Users
{
    public static class UserMap
    {
        public static ModelBuilder MapUser(this ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<User>();

            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Email).IsRequired();

            return modelBuilder;
        }
    }
}
