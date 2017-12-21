using Serenity;
using System.Web;
using System.Web.Mvc;

namespace Test.Membership
{
    /// <summary>
    /// Limit one access per user 
    /// When the same user access in another location, the prev user is disconnected
    /// </summary>
    public class LoginLimitAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (ctx.Session["sessionid"] == null)
                ctx.Session["sessionid"] = "empty";

            // check to see if your ID in the Logins table has LoggedIn = true 
            // if so, continue, otherwise, redirect to Login page.
            var userName = Authorization.Username;
            if (LoginLimit.IsYourLoginStillTrue(userName, ctx.Session["sessionid"].ToString()))
            {
                // Check to see if your user ID is being used elsewhere under a different session ID
                if (!LoginLimit.IsUserLoggedOnElsewhere(userName, ctx.Session["sessionid"].ToString()))
                {
                    // Here the user has only access from this session, then OK
                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    // Here the user has other access. 
                    // Then ask you if you want force logout on other position.

                    // Force Logout indicate if came from LoginConfirmation page to force logout
                    var forceLogout = ctx.Session["forceLogout"];
                    if (forceLogout == null)
                    {
                        // The user has attempted to access, but there is same user
                        // logged somewhere. Remove loginlimit record created in login
                        // to simulate the his never logged
                        LoginLimit.SignOut();
                        // Redirect to confirm force logout page
                        filterContext.Result = new RedirectResult("~/Account/LoginConfirm");
                        return;
                    }
                    else
                    {
                        // if it is being used elsewhere, update all their Logins 
                        // records to LoggedIn = false, except for your session ID
                        LoginLimit.LogEveryoneElseOut(userName, ctx.Session["sessionid"].ToString());
                        ctx.Session["forceLogout"] = null;
                        base.OnActionExecuting(filterContext);
                    }
                }
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Account/Signout");
                return;
            }
        }
    }
}
