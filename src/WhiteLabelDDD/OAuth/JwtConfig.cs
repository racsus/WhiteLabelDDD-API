using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WhiteLabel.Application.Configuration;

namespace WhiteLabelDDD.OAuth
{
    internal static class JwtConfig
    {
        public static void ConfigureServices(IServiceCollection services, AuthConfiguration authConfiguration)
        {
            if (authConfiguration?.IsEnabled == true)
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = authConfiguration.Authority;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidIssuers = new[]
                            {
                                authConfiguration.Authority + "/v2.0"
                            },
                            ValidAudiences = new[]
                            {
                                authConfiguration.Authority + "/resources",
                                authConfiguration.Application
                            },
                        };

#if DEBUG

                        options.RequireHttpsMetadata = false;
                        options.IncludeErrorDetails = true;

                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = AuthenticationFailed
                        };

                    #endif

                    });
            }
        }

        public static void Configure(IApplicationBuilder app, AuthConfiguration authConfiguration)
        {
            if (authConfiguration?.IsEnabled == true)
            {
                app.UseAuthentication();
            }
        }

        private static Task AuthenticationFailed(AuthenticationFailedContext arg)
        {
            // For debugging purposes only!
            var s = $"AuthenticationFailed: {arg.Exception.Message}";
            arg.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            arg.Response.ContentLength = s.Length;
            arg.Response.Body.Write(Encoding.UTF8.GetBytes(s), 0, s.Length);
            return Task.FromResult(0);
        }
    }
}