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
        private readonly IAuthorizationService _authorization;
        private readonly AuthConfiguration _authConfiguration;
        public string Policy { get; private set; }

        public AuthorizeWithPermissionsFilter(string policy, IAuthorizationService authorization, AuthConfiguration authConfiguration)
        {
            this.Policy = policy;
            _authorization = authorization;
            _authConfiguration = authConfiguration;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (_authConfiguration.AuthType.ToUpper() == AuthConstants.Auth0)
            {
                var permissions = context.HttpContext.User.Claims.Where(x => x.Type == "permissions")?.Select(x => x.Value).ToList();
                var authorized = permissions.Contains(this.Policy);
                if (!authorized)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            } else if (_authConfiguration.AuthType.ToUpper() == AuthConstants.Azure)
            {
                var authorized = await _authorization.AuthorizeAsync(context.HttpContext.User, this.Policy);
                if (!authorized.Succeeded)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }
}
