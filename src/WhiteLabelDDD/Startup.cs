using Autofac;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WhiteLabel.Application.Configuration;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Infrastructure.DependencyInjection;
using WhiteLabel.WebAPI.Exceptions;
using WhiteLabel.WebAPI.Filters;
using WhiteLabel.WebAPI.OAuth;
using WhiteLabel.WebAPI.Swagger;

namespace WhiteLabel.WebAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private AuthConfiguration AuthConfiguration { get; }
        private SwaggerConfiguration SwaggerConfiguration { get; }
        private ApiMetadataConfiguration ApiMetadata { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AuthConfiguration = configuration
                .GetSection(AuthConfiguration.Section)
                .Get<AuthConfiguration>();
            SwaggerConfiguration = configuration
                .GetSection(SwaggerConfiguration.Section)
                .Get<SwaggerConfiguration>();
            ApiMetadata = configuration
                .GetSection(ApiMetadataConfiguration.Section)
                .Get<ApiMetadataConfiguration>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtConfig.ConfigureServices(services, AuthConfiguration);

            #region Allow-Orgin

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
            });

            #endregion

            services.AddControllers();
            // Fluent Validation
            services.AddControllers(options =>
            {
                options.Filters.Add(new ValidationFilter());
            });
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<UserDto>();

            // Add API Versioning to as service to your project
            services.AddApiVersioning(config =>
            {
                // Specify the default API Version as 1.0
                config.DefaultApiVersion = new ApiVersion(1, 0);
                // If the client hasn't specified the API version in the request, use the default API version number
                config.AssumeDefaultVersionWhenUnspecified = true;
                // Advertise the API versions supported for the particular endpoint
                config.ReportApiVersions = true;
                // Supporting multiple versioning scheme
                config.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("X-version"),
                    new QueryStringApiVersionReader("api-version")
                );
            });

            // Add Swagger services
            SwaggerConfig.ConfigureServices(services, AuthConfiguration, ApiMetadata);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            ContainerSetup.Initialize(builder, Configuration, AuthConfiguration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            // global error handler
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseHttpsRedirection();

            JwtConfig.Configure(app, AuthConfiguration);

            // Configure Swagger
            SwaggerConfig.Configure(app, SwaggerConfiguration, AuthConfiguration, ApiMetadata);

            // CORS
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseRouting();

            if (AuthConfiguration?.IsEnabled == true)
            {
                app.UseAuthentication();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers().RequireAuthorization();
                });
            }
            else
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
        }
    }
}
