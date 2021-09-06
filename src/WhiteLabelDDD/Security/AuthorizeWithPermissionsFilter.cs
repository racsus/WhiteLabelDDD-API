﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WhiteLabel.Application.Configuration;
using WhiteLabel.WebAPI.OAuth;

namespace eifhub.WebAPI.Security
{
    public class AuthorizeWithPermissionsFilter : IAsyncAuthorizationFilter
    {
        private readonly IAuthorizationService _authorization;
        private readonly AuthConfiguration _authConfiguration;
        public string _policy { get; private set; }

        public AuthorizeWithPermissionsFilter(string policy, IAuthorizationService authorization, AuthConfiguration authConfiguration)
        {
            _policy = policy;
            _authorization = authorization;
            _authConfiguration = authConfiguration;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (_authConfiguration.AuthType.ToUpper() == AuthConstants.Auth0)
            {
                var permissions = context.HttpContext.User.Claims.Where(x => x.Type == "permissions")?.Select(x => x.Value).ToList();
                var authorized = permissions.Contains(_policy);
                if (!authorized)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            } else if (_authConfiguration.AuthType.ToUpper() == AuthConstants.Azure)
            {
                var authorized = await _authorization.AuthorizeAsync(context.HttpContext.User, _policy);
                if (!authorized.Succeeded)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }
}
