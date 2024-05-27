using Microsoft.AspNetCore.Mvc.Filters;

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
