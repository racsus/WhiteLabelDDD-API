using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using WhiteLabel.ConvertToYeoman.Helpers;
using WhiteLabel.ConvertToYeoman.Services;

namespace WhiteLabel.ConvertToYeoman
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();

            var errorMessage = ArgValidatorHelper.Check(args);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ConsoleLogHelper.ShowInfoMessage(errorMessage, ConsoleColor.Red);
                ConsoleLogHelper.WaitForUser();
                return;
            }

            // calls the Run method in App, which is replacing Main
            serviceProvider.GetService<YeomanService>().Migrate(args[0]);
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var config = LoadConfiguration();
            services.AddSingleton(config);

            // required to run the application
            services.AddScoped<YeomanService>();

            return services;
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }
    }
}
