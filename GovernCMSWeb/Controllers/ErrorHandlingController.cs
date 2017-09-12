using System;
using System.Security.Authentication;
using System.Web.Mvc;
using GovernCMS.Models;
using GovernCMS.Utils;
using log4net;

namespace GovernCMS.Controllers
{
    public abstract class ErrorHandlingController : Controller
    {

        protected ILog Logger
        {
            get { return LogManager.GetLogger(GetType()); }
        }

        // Error Handling
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            String errorMessage = filterContext.Exception.Message;

            Logger.Error(String.Format("AuthenticatedController handling error with type: {0} and message: {1}",
                filterContext.Exception.GetType().FullName, errorMessage), filterContext.Exception);
            if (filterContext.Exception.InnerException != null)
            {
                Logger.Error("InnerException: " + filterContext.Exception.InnerException.Message, filterContext.Exception.InnerException);
            }
            // Redirect on error:
            TempData["errorMessage"] = errorMessage;

            if (filterContext.Exception is AuthenticationException)
            {
                string originalUrl = HttpContext.Request.RawUrl;

                filterContext.Result = RedirectToAction("Login", "User", new {redirectUrl = originalUrl});
            }
            else
            {
                filterContext.Result = RedirectToAction("Index", "Home");
            }
        }

        protected void UserCheck()
        {
            User currentUser = (User)Session[Constants.CURRENT_USER];

            // Guard blocks
            if (currentUser == null)
            {
                throw new AuthenticationException("You must be logged in to perform that action.");
            }
        }
    }
}