namespace Test.Common.Pages
{
    using Serenity;
    using SpeedWE.Membership;
    using System.Web.Mvc;

    [RoutePrefix("Dashboard"), Route("{action=index}")]
    [LoginLimit] // This is the ActionFilter to limit access
    public class DashboardController : Controller
    {
        [Authorize, HttpGet, Route("~/")]
        public ActionResult Index()
        {
            return View(MVC.Views.Common.Dashboard.DashboardIndex);
        }
    }
}
