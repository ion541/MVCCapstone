using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCCapstone.Models
{

    /// <summary>
    /// Attribute used to redirect users to an error page if they try and access an ajax link directly through the url
    /// </summary>
    public class AjaxAction : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "IndirectAccess" }));
            }
        }
    }

    /// <summary>
    /// Attribute that inherits the Authorize attribute.
    /// 
    /// If the user is not logged in, it will redirect the user to the log in page.
    /// 
    /// If the user is logged in, it will redirect the user to the  error page instead of always redirecting to the Log in page in the event of unauthorization
    /// The process above is repeated when the user logs in and doesn't have the authority to access it.
    /// </summary>
    public class RoleAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // test to see if the user is logged in
            if(filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // redirect to error page
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "UnauthorizedAccess" }));
            }
            else
            {
                // redirect to login page
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}