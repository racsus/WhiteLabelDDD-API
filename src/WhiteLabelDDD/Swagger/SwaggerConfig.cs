using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using WhiteLabel.Application.Configuration;
using WhiteLabel.WebAPI.Swagger;
using WhiteLabelDDD.OAuth;

namespace WhiteLabelDDD.Swagger
{
    /// <summary>
    /// Swagger configuration
    /// </summary>
    internal static class SwaggerConfig
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add Swagger services to the container.
        /// </summary>
        /// <param name="services">services to configure</param>
        /// <param name="authConfiguration">auth Configuration</param>
        /// <param name="apiVersion">api version</param>
        public static void ConfigureServices(IServiceCollection services, AuthConfiguration authConfiguration, ApiMetadataConfiguration apiMetaData)
        {
            services.AddSwaggerGen(c =>
            {
                foreach (ApiVersionData apiVersionData in apiMetaData.ApiVersions)
                {
                    var openApiInfo = new OpenApiInfo
                    {
                        Version = apiVersionData.Version,
                        Title = apiVersionData.Title,
                        Description = apiVersionData.Description,
                        TermsOfService = !string.IsNullOrEmpty(apiVersionData.TermsOfService) ? new Uri(apiVersionData.TermsOfService) : null
                    };

                    if (apiMetaData.ApiContactData != null)
                    {
                        openApiInfo.Contact = new OpenApiContact
                        {
                            Name = apiMetaData.ApiContactData.Name,
                            Email = apiMetaData.ApiContactData.Email,
                            Url = !string.IsNullOrEmpty(apiMetaData.ApiContactData.Url) ? new Uri(apiMetaData.ApiContactData.Url) : null,
                        };
                    }

                    c.SwaggerDoc(apiVersionData.Version, openApiInfo);
                }
                if (authConfiguration?.IsEnabled == true)
                {
                    // Azure
                    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri($"{authConfiguration.Authority}/oauth2/v2.0/authorize"),
                                TokenUrl = new Uri($"{authConfiguration.Authority}/oauth2/v2.0/token"),
                                Scopes = new Dictionary<string, string>
                                        {
                                            { $"{authConfiguration.Scope}", authConfiguration.Scope }
                                        }
                            }
                        }
                    });
                    // Auth0
                    //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    //{
                    //    Name = "Authorization",
                    //    In = ParameterLocation.Header,
                    //    Type = SecuritySchemeType.OAuth2,
                    //    Flows = new OpenApiOAuthFlows
                    //    {
                    //        Implicit = new OpenApiOAuthFlow
                    //        {
                    //            Scopes = new Dictionary<string, string>
                    //            {
                    //                { "openid", "Open Id" }
                    //            },
                    //            AuthorizationUrl = new Uri("https://ecritportfolio.eu.auth0.com/authorize?https://budgeting-tool-api")
                    //        }
                    //    }
                    //});
                    c.OperationFilter<SecurityRequirementsOperationFilter>();
                }

                // Apply the filters
                c.OperationFilter<RemoveVersionFromParameter>();
                c.DocumentFilter<ReplaceVersionWithExactValueInPath>();
            });

            services.AddSwaggerGenNewtonsoftSupport();
        }

        /// <summary>
        /// Configure application
        /// </summary>
        /// <param name="app">Application to configure</param>
        /// <param name="swaggerConfiguration">swagger Configuration</param>
        /// <param name="authConfiguration">auth Configuration</param>
        /// <param name="apiVersion">api version</param>
        public static void Configure(IApplicationBuilder app, SwaggerConfiguration swaggerConfiguration, AuthConfiguration authConfiguration, ApiMetadataConfiguration apiMetaData)
        {
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
                });
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                foreach (ApiVersionData apiVersionData in apiMetaData.ApiVersions)
                {
                    c.SwaggerEndpoint($"/swagger/{apiVersionData.Version}/swagger.json", $"{apiVersionData.Version} Docs");
                }                
                c.RoutePrefix = string.Empty;
                c.DisplayRequestDuration();

                if (authConfiguration?.IsEnabled == true)
                {
                    c.OAuthClientId(swaggerConfiguration.ClientId);
                    c.OAuthRealm(authConfiguration.Application);
                    c.OAuthAppName(swaggerConfiguration.ApplicationName);
                    c.OAuthScopeSeparator(" ");
                }
            });
        }

    }
}
