using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System.Linq;
using System.Threading.Tasks;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Application.Interfaces.Generic;
using WhiteLabel.Application.Interfaces.Users;

namespace WhiteLabel.WebAPI.Controllers.Generic
{
    public abstract class WhiteLabelController<T>(IUserService userService, T businessService) : Controller
        where T : IBusinessService
    {
        protected readonly T BusinessService = businessService;
        protected UserInfoDto UserInfoDto;

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        )
        {
            var hasNeedsUserFilter =
                context.Filters.Any(x => x.ToString()!.Contains("WebAPI.Security.NeedsUserFilter"));
            if (hasNeedsUserFilter)
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];
                UserInfoDto = await userService.GetUserInfo(accessToken, User);
            }

            await next();
        }
    }
}
