using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Application.Configuration;
using WhiteLabel.Application.Constants;

namespace WhiteLabel.WebAPI.Security
{
    public class AuthorizeWithPermissionsFilter : IAsyncAuthorizationFilter
    {
        private readonly IAuthorizationService authorization;
        private readonly AuthConfiguration authConfiguration;
        private string Policy { get; set; }

        public AuthorizeWithPermissionsFilter(
            string policy,
            IAuthorizationService authorization,
            AuthConfiguration authConfiguration
        )
        {
            Policy = policy;
            this.authorization = authorization;
            this.authConfiguration = authConfiguration;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (authConfiguration.AuthType.ToUpper() == AuthConstants.Auth0)
            {
                var permissions = context.HttpContext.User.Claims
                    .Where(x => x.Type == "permissions")
                    .Select(x => x.Value)
                    .ToList();
                var authorized = permissions.Contains(Policy);
                if (!authorized)
                {
                    context.Result = new ForbidResult();
                }
            }
            else if (authConfiguration.AuthType.ToUpper() == AuthConstants.Azure)
            {
                var authorized = await authorization.AuthorizeAsync(
                    context.HttpContext.User,
                    Policy
                );
                if (!authorized.Succeeded)
                {
                    context.Result = new ForbidResult();
                }
            }
            else if (authConfiguration.AuthType.ToUpper() == AuthConstants.Database)
            {
                var authorized = await authorization.AuthorizeAsync(
                    context.HttpContext.User,
                    Policy
                );
                if (!authorized.Succeeded)
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}
