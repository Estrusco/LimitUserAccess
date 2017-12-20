
namespace Test.Membership.Pages
{
    using Serenity;
    using Serenity.Services;
    using System;
    using System.Web.Mvc;
    using System.Web.Security;

    [RoutePrefix("Account"), Route("{action=index}")]
    public partial class AccountController : Controller
    {
        public static bool UseAdminLTELoginBox = false;

        [HttpGet]
        public ActionResult Login(string activated)
        {
            ViewData["Activated"] = activated;
            ViewData["HideLeftNavigation"] = true;

            if (UseAdminLTELoginBox)
                return View(MVC.Views.Membership.Account.AccountLogin_AdminLTE);
            else
                return View(MVC.Views.Membership.Account.AccountLogin);
        }

        [HttpPost, JsonFilter]
        public Result<ServiceResponse> Login(LoginRequest request)
        {
            return this.ExecuteMethod(() =>
            {
                request.CheckNotNull();

                if (string.IsNullOrEmpty(request.Username))
                    throw new ArgumentNullException("username");

                var username = request.Username;

                if (WebSecurityHelper.Authenticate(ref username, request.Password, false))
                {
                    #region Limit User
                    var entity = new LoginRow();
                    entity.UserName = username;
                    entity.SessionId = System.Web.HttpContext.Current.Session.SessionID;
                    entity.LoggedIn = true;

                    new LoginRepository().CreateOrUpdate(entity);

                    Session["sessionid"] = System.Web.HttpContext.Current.Session.SessionID;
                    #endregion

                    return new ServiceResponse();
                }

                throw new ValidationError("AuthenticationError", Texts.Validation.AuthenticationError);
            });
        }

        private ActionResult Error(string message)
        {
            return View(MVC.Views.Errors.ValidationError,
                new ValidationError(Texts.Validation.InvalidResetToken));
        }

        public ActionResult Signout()
        {
            #region Limit User
            var entity = new LoginRow();
            entity.UserName = Authorization.Username;
            entity.SessionId = System.Web.HttpContext.Current.Session.SessionID;
            new LoginRepository().Delete(entity); 
            #endregion

            Session.Abandon();
            FormsAuthentication.SignOut();
            return new RedirectResult("~/");
        }
    }
}
