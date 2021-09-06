using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WhiteLabel.Application.Configuration;
using WhiteLabel.WebAPI.OAuth;
using WhiteLabelDDD.OAuth;

namespace WhiteLabelDDD.Swagger
{
    internal class SecurityRequirementsOperationFilter : IOperationFilter
    {
        private readonly IOptions<AuthorizationOptions> authorizationOptions;
        private readonly AuthConfiguration authConfiguration;

        public SecurityRequirementsOperationFilter(IOptions<AuthorizationOptions> authorizationOptions, AuthConfiguration authConfiguration)
        {
            this.authorizationOptions = authorizationOptions;
            this.authConfiguration = authConfiguration;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var securitySchemeId = string.Empty;
            switch (authConfiguration.AuthType.ToUpper())
            {
                case AuthConstants.Azure:
                    securitySchemeId = "oauth2";
                    break;
                case AuthConstants.Auth0:
                case AuthConstants.Bearer:
                    securitySchemeId = "Bearer";
                    break;
            }
            var controllerPolicies = context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Select(attr => attr.Policy);
            var actionPolicies = context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Select(attr => attr.Policy);
            var policies = controllerPolicies.Union(actionPolicies).Distinct();
            var requiredClaimTypes = policies
                .Select<string, AuthorizationPolicy>(x => this.authorizationOptions.Value.GetPolicy(x))
                .SelectMany(x => x.Requirements)
                .OfType<ClaimsAuthorizationRequirement>()
                .Select(x => x.ClaimType);

            if ((requiredClaimTypes.Any() || this.authConfiguration.IsEnabled) && (!string.IsNullOrEmpty(securitySchemeId)))
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = securitySchemeId }
                                },
                                new[] { "readAccess", "writeAccess" }
                            }
                        }
                };
            }
        }
    }
}