using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WhiteLabel.Application.Configuration;
using WhiteLabel.Application.Constants;
using WhiteLabel.WebAPI.Security;

namespace WhiteLabelDDD.OAuth
{
    internal static class JwtConfig
    {
        public static void ConfigureServices(IServiceCollection services, AuthConfiguration authConfiguration)
        {
            if (authConfiguration?.IsEnabled == true)
            {
                if (authConfiguration.AuthType.ToUpper() == AuthConstants.Database)
                {
                    services.AddAuthentication("BasicAuthentication")
                        .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
                }
                else
                {
                    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            options.Authority = authConfiguration.Authority;
                            options.Audience = authConfiguration.Audience;
                            // If the access token does not have a `sub` claim, `User.Identity.Name` will be `null`. Map it to a different claim by setting the NameClaimType below.
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                NameClaimType = ClaimTypes.NameIdentifier
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
        }

        public static void Configure(IApplicationBuilder app, AuthConfiguration authConfiguration)
        {

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