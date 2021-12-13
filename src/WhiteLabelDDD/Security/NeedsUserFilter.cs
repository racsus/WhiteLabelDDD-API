using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WhiteLabel.WebAPI.Security
{
    public class NeedsUserFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // our code before action executes
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // our code before action executes
        }
    }
}
