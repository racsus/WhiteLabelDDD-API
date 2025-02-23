using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
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

namespace WhiteLabel.WebAPI.OAuth
{
    internal static class JwtConfig
    {
        public static void ConfigureServices(
            IServiceCollection services,
            AuthConfiguration authConfiguration
        )
        {
            if (authConfiguration?.Enabled == true)
            {
                if (authConfiguration.AuthType.ToUpper() == AuthConstants.Database)
                    services
                        .AddAuthentication("BasicAuthentication")
                        .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
                            "BasicAuthentication",
                            null
                        );
                else if (authConfiguration.AuthType.ToUpper() == AuthConstants.Bearer)
                    // https://medium.com/@levanrevazashvili/jwt-and-refresh-tokens-in-asp-net-core-11a877575147
                    // https://github.com/Revazashvili/UserManagement
                    services
                        .AddAuthentication(x =>
                        {
                            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        })
                        .AddJwtBearer(x =>
                        {
                            x.SaveToken = true;
                            x.RequireHttpsMetadata = false;
                            x.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuerSigningKey = true,
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                IssuerSigningKey = new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(authConfiguration.AccessTokenSecret)
                                ),
                                ValidIssuer = authConfiguration.Issuer,
                                ValidAudience = authConfiguration.Audience,
                            };
                        });
                else
                    services
                        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            options.Authority = authConfiguration.Authority;
                            options.Audience = authConfiguration.Audience;
                            // If the access token does not have a `sub` claim, `User.Identity.Name` will be `null`. Map it to a different claim by setting the NameClaimType below.
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                NameClaimType = ClaimTypes.NameIdentifier,
                            };

#if DEBUG

                            options.RequireHttpsMetadata = false;
                            options.IncludeErrorDetails = true;

                            options.Events = new JwtBearerEvents
                            {
                                OnAuthenticationFailed = AuthenticationFailed,
                            };
#endif
                        });
            }
        }

        public static string Generate(
            string secretKey,
            string issuer,
            string audience,
            double expires,
            IEnumerable<Claim> claims = null
        )
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken securityToken = new(
                issuer,
                audience,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(expires),
                credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        public static void Configure(
            IApplicationBuilder app,
            AuthConfiguration authConfiguration
        ) { }

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
