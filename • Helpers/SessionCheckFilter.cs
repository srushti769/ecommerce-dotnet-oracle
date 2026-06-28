using System.Web;
using System.Web.Mvc;

namespace ECommerceApp.Filters
{
    public class AdminOnly : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["UserRole"] == null ||
                HttpContext.Current.Session["UserRole"].ToString() != "Admin")
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class UserOnly : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["UserId"] == null)
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
