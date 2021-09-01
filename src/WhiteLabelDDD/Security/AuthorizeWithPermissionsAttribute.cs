using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eifhub.WebAPI.Security
{
    public class AuthorizeWithPermissionsAttribute : TypeFilterAttribute, IAllowAnonymous
    {
        public AuthorizeWithPermissionsAttribute(string policy) : base(typeof(AuthorizeWithPermissionsFilter))
        {
            Arguments = new object[] { policy };
        }
    }
}
