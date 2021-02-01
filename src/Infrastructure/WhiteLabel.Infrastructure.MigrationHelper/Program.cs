using System;

namespace WhiteLabel.Infrastructure.MigrationHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new DbContextFactory();
            var dbContext = factory.CreateDbContext(System.Array.Empty<string>());
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }
    }
}
