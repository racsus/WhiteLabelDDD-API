using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;
using WhiteLabel.Infrastructure.Data;

namespace WhiteLabel.Infrastructure.MigrationHelper
{
    public class DbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        private const string ConnectionString =
            "Data Source=localhost\\SQLEXPRESS;Initial Catalog=WhiteLabelDDD;Integrated Security=True;";

        public AppDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseSqlServer(
                ConnectionString,
                b => b.MigrationsAssembly("WhiteLabel.Infrastructure.Data")
            );

            return new AppDbContext(builder.Options, null);
        }
    }
}
