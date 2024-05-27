using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace WhiteLabel.Infrastructure.Data
{
    public static class StartupSetup
    {
        public static void AddDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        }
    }
}
