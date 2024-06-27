using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace HealthifyConsume.BAL
{
    public class CheckAccess : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var rd = filterContext.RouteData;

            string currentAction = rd.Values["action"].ToString();
            string currentController = rd.Values["controller"].ToString();
            //string currentArea = rd.DataTokens["area"].ToString()??" ";

            //Console.WriteLine(rd +" " +currentAction+" "+  currentController );
            if ((filterContext.HttpContext.Session.GetInt32("UserID") == null || filterContext.HttpContext.Session.GetString("UserName") == null))
            {
                filterContext.HttpContext.Session.Clear();
                filterContext.Result = new RedirectResult("~/Login/Register/Login");
            }


        }

		public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            filterContext.HttpContext.Response.Headers["Expires"] = "-1";
            filterContext.HttpContext.Response.Headers["Pragma"] = "no-cache";

            base.OnResultExecuting(filterContext);
        }


    }
}
