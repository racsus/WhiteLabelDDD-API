﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using WhiteLabel.Application.Configuration;
using WhiteLabel.Application.Constants;

namespace WhiteLabel.WebAPI.Swagger
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
        /// <param name="apiMetaData"></param>
        public static void ConfigureServices(
            IServiceCollection services,
            AuthConfiguration authConfiguration,
            ApiMetadataConfiguration apiMetaData
        )
        {
            services.AddSwaggerGen(c =>
            {
                foreach (var apiVersionData in apiMetaData.ApiVersions)
                {
                    var openApiInfo = new OpenApiInfo
                    {
                        Version = apiVersionData.Version,
                        Title = apiVersionData.Title,
                        Description = apiVersionData.Description,
                        TermsOfService = !string.IsNullOrEmpty(apiVersionData.TermsOfService)
                            ? new Uri(apiVersionData.TermsOfService)
                            : null,
                    };

                    if (apiMetaData.ApiContactData != null)
                        openApiInfo.Contact = new OpenApiContact
                        {
                            Name = apiMetaData.ApiContactData.Name,
                            Email = apiMetaData.ApiContactData.Email,
                            Url = !string.IsNullOrEmpty(apiMetaData.ApiContactData.Url)
                                ? new Uri(apiMetaData.ApiContactData.Url)
                                : null,
                        };

                    c.SwaggerDoc(apiVersionData.Version, openApiInfo);
                }

                if (authConfiguration?.Enabled == true)
                {
                    if (authConfiguration.AuthType.ToUpper() == AuthConstants.Azure)
                    {
                        // Azure
                        c.AddSecurityDefinition(
                            "oauth2",
                            new OpenApiSecurityScheme
                            {
                                Type = SecuritySchemeType.OAuth2,
                                Flows = new OpenApiOAuthFlows
                                {
                                    Implicit = new OpenApiOAuthFlow
                                    {
                                        AuthorizationUrl = new Uri(
                                            $"{authConfiguration.Authority}/oauth2/v2.0/authorize"
                                        ),
                                        TokenUrl = new Uri(
                                            $"{authConfiguration.Authority}/oauth2/v2.0/token"
                                        ),
                                        Scopes = new Dictionary<string, string>
                                        {
                                            {
                                                $"{authConfiguration.Scope}",
                                                authConfiguration.Scope
                                            },
                                        },
                                    },
                                },
                            }
                        );
                    }
                    else if (authConfiguration.AuthType.ToUpper() == AuthConstants.Auth0)
                    {
                        // Auth0
                        c.AddSecurityDefinition(
                            "Bearer",
                            new OpenApiSecurityScheme
                            {
                                Name = "Authorization",
                                In = ParameterLocation.Header,
                                Type = SecuritySchemeType.OAuth2,
                                Flows = new OpenApiOAuthFlows
                                {
                                    Implicit = new OpenApiOAuthFlow
                                    {
                                        Scopes = new Dictionary<string, string>
                                        {
                                            { "openid", "openid" },
                                            { "email", "email" },
                                            { "profile", "profile" },
                                        },
                                        AuthorizationUrl = new Uri(
                                            $"{authConfiguration.Authority}/authorize?audience={authConfiguration.Audience}"
                                        ),
                                    },
                                },
                            }
                        );
                    }
                    else if (authConfiguration.AuthType.ToUpper() == AuthConstants.Bearer)
                    {
                        // Bearer
                        c.AddSecurityDefinition(
                            JwtBearerDefaults.AuthenticationScheme,
                            new OpenApiSecurityScheme
                            {
                                Description =
                                    "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                                Name = "Authorization",
                                In = ParameterLocation.Header,
                                Type = SecuritySchemeType.ApiKey,
                                Scheme = JwtBearerDefaults.AuthenticationScheme,
                            }
                        );

                        c.AddSecurityRequirement(
                            new OpenApiSecurityRequirement
                            {
                                {
                                    new OpenApiSecurityScheme
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.SecurityScheme,
                                            Id = JwtBearerDefaults.AuthenticationScheme,
                                        },
                                    },
                                    Array.Empty<string>()
                                },
                            }
                        );
                    }
                    else if (authConfiguration.AuthType.ToUpper() == AuthConstants.Database)
                    {
                        // Database
                        c.AddSecurityDefinition(
                            "basic",
                            new OpenApiSecurityScheme
                            {
                                Name = "Authorization",
                                Type = SecuritySchemeType.Http,
                                Scheme = "basic",
                                In = ParameterLocation.Header,
                                Description = "Basic Authorization header using the Bearer scheme.",
                            }
                        );
                        c.AddSecurityRequirement(
                            new OpenApiSecurityRequirement
                            {
                                {
                                    new OpenApiSecurityScheme
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.SecurityScheme,
                                            Id = "basic",
                                        },
                                    },
                                    new string[] { }
                                },
                            }
                        );
                    }

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
        /// <param name="apiMetaData"></param>
        public static void Configure(
            IApplicationBuilder app,
            SwaggerConfiguration swaggerConfiguration,
            AuthConfiguration authConfiguration,
            ApiMetadataConfiguration apiMetaData
        )
        {
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add(
                    (swagger, httpReq) =>
                    {
                        swagger.Servers = new List<OpenApiServer>
                        {
                            new() { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" },
                        };
                    }
                );
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                foreach (var apiVersionData in apiMetaData.ApiVersions)
                    c.SwaggerEndpoint(
                        $"/swagger/{apiVersionData.Version}/swagger.json",
                        $"{apiVersionData.Version} Docs"
                    );
                c.RoutePrefix = string.Empty;
                c.DisplayRequestDuration();

                if (authConfiguration?.Enabled == true)
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
