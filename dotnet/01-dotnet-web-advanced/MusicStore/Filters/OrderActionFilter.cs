using Microsoft.AspNetCore.Mvc.Filters;

namespace MusicStore.Filters
{
    public class OrderActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // TODO
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {

            //Do something before process
            var user = filterContext.HttpContext.User;
           if(user.Identity == null)
            {
                //To do somthing ì don't have identity
            }
        }
    }
}
