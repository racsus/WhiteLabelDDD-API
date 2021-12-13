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
    public abstract class WhiteLabelController<T> : Controller where T : IBusinessService
    {
        protected readonly IUserService userService;
        protected readonly T businessService;
        protected UserInfoDTO user;

        public WhiteLabelController(IUserService userService, T businessService)
        {
            this.userService = userService;
            this.businessService = businessService;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var hasNeedsUserFilter = context.Filters.Where(x => x.ToString().Contains("WebAPI.Security.NeedsUserFilter")).Count() > 0;
            if (hasNeedsUserFilter)
            {
                var accessToken = Request.Headers[HeaderNames.Authorization];
                user = await this.userService.GetUserInfo(accessToken, this.User);
            }

            await next();
        }

    }
}
