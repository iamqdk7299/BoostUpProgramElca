using Microsoft.AspNetCore.Mvc.Filters;
using MusicStore.Models;

namespace MusicStore.Filters
{
    public class AuditTrailsActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            ActionLog log = new ActionLog()
            {
                Controller = filterContext.ActionDescriptor.DisplayName,
                Action = string.Format("{0} {1} (Logged By: CustomActionFilter)", filterContext.HttpContext.Request.Method, filterContext.ActionDescriptor.DisplayName)
            };
            // Save log to DB or log file
        }
    }
}
