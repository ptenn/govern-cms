using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GovernCMS.Models;
using GovernCMS.Services;
using GovernCMS.Services.Impl;
using GovernCMS.Utils;
using GovernCMS.ViewModels;
using log4net;

namespace GovernCMS.Controllers
{
    public class UserController : ErrorHandlingController
    {
        private static ILog logger = LogManager.GetLogger(typeof(UserController));

        private GovernCmsContext db = new GovernCmsContext();

        private IUserService userService;

        private IOrganizationService organizationService;

        public UserController()
        {
            userService = new UserService();
            organizationService = new OrganizationService();
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateUserViewModel createUserViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {


                    User user = userService.CreateUser(createUserViewModel.EmailAddr,
                        createUserViewModel.Passwd,
                        createUserViewModel.ConfirmPasswd,
                        createUserViewModel.FirstName,
                        createUserViewModel.LastName,
                        createUserViewModel.OrganizationId,
                        createUserViewModel.OrganizationName);
                    // Put User in the session
                    Session[Constants.CURRENT_USER] = user;

                    // Record LoginViewModel Attempt
                    RecordLoginAttempt(user, Request);

                    logger.Debug("Created User " + user.UserId);
                    TempData["successMessage"] = $"User {user.FirstName} {user.LastName} successfully created";

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    TempData["errorMessage"] = e.Message;
                    return View(createUserViewModel);
                }

            }

            return View(createUserViewModel);
        }

        // Get: User/LoginViewModel
        public ActionResult Login(string redirectUrl)
        {
            LoginViewModel loginViewModel = new LoginViewModel()
            {
                RedirectUrl = redirectUrl
            };
            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel loginViewModel)
        {
            // First, find the Real User based on the email address
            string cleanEmailAddr = StringUtils.CleanEmailAddr(loginViewModel.EmailAddr);

            // There should only be one result due to AK on CleanEmailAddr column in DB
            User user = db.Users
                .Include(u => u.Organization)
                .FirstOrDefault(u => u.EmailAddr == cleanEmailAddr);

            if (user == null)
            {
                ModelState.AddModelError("ErrorMessage", "Email Address or Password was invalid");
                return View();
            }

            var encrypedPass = PasswordUtils.Sha256(loginViewModel.Passwd + user.Salt);

            // user entered correct password
            if (encrypedPass.Equals(user.Passwd))
            {
                // LoginViewModel successful, Put User in the session
                Session[Constants.CURRENT_USER] = user;

                // Record LoginViewModel Attempt
                RecordLoginAttempt(user, Request);

                TempData["successMessage"] = $"User {user.FirstName} {user.LastName} successfully logged in";

                if (!string.IsNullOrEmpty(loginViewModel.RedirectUrl))
                {
                    return new RedirectResult(loginViewModel.RedirectUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("ErrorMessage", "Email Address or Password was invalid");
            return View();
        }

        public ActionResult Logout()
        {
            // Clear out session
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Edit()
        {
            // Guard block
            UserCheck();

            User currentUser = (User) Session[Constants.CURRENT_USER];

            User user = userService.FindUserById(currentUser.UserId);
            if (user != null)
            {
                CreateUserViewModel createUserViewModel = new CreateUserViewModel();
                createUserViewModel.UserId = user.UserId;
                createUserViewModel.EmailAddr = user.EmailAddr;
                createUserViewModel.FirstName = user.FirstName;
                createUserViewModel.LastName = user.LastName;

                return View(createUserViewModel);
            }
            TempData["errorMessage"] = "Unable to Edit User";
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(CreateUserViewModel createUserViewModel)
        {
            try
            {
                User user = userService.EditUser(createUserViewModel.UserId, createUserViewModel.Passwd,
                    createUserViewModel.ConfirmPasswd,
                    createUserViewModel.FirstName, createUserViewModel.LastName);

                // Put Updated User in the session
                Session[Constants.CURRENT_USER] = user;

                TempData["successMessage"] = "User Successfully Updated";
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                TempData["errorMessage"] = "Unable to update User";
            }            
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public JsonResult FindUsers(string term)
        {            
            UserCheck();
            User currentUser = (User)Session[Constants.CURRENT_USER];

            IList<User> users = userService.FindUsers(term, currentUser.OrganizationId);
            
            IList<IDictionary<string, object>> autoCompleteUsers = new List<IDictionary<string, object>>();
            foreach (User user in users)
            {
                IDictionary<string, object> autoCompleteUser = new Dictionary<string, object>();

                autoCompleteUser.Add("Id", user.UserId);
                autoCompleteUser.Add("Name", user.FirstName + " " + user.LastName);
                autoCompleteUsers.Add(autoCompleteUser);
            }
            return Json(autoCompleteUsers);
        }

        [HttpPost]
        public JsonResult FindOrganizationByEmail(string emailAddr)
        {
            OrgSearchResultViewModel searchResultViewModel = new OrgSearchResultViewModel();

            Organization organization = organizationService.FindOrganizationByEmailAddr(emailAddr);
            if (organization != null)
            {
                searchResultViewModel.Match = true;
                searchResultViewModel.OrgId = organization.OrganizationId;
                searchResultViewModel.OrgName = organization.Name;
            }
            else
            {
                searchResultViewModel.Match = false;
                searchResultViewModel.AllOrgNames = db.Organizations.Select(o => o.Name).ToList();
            }
            return Json(searchResultViewModel);
        }

        #region Helper Methods
        private void RecordLoginAttempt(User user, HttpRequestBase request)
        {
            // Grab the Request ServerVariables and store them for this LoginAttempt
            StringBuilder requestInfo = new StringBuilder();
            string delimiter = "";
            LoginAttempt loginAttempt = new LoginAttempt();
            foreach (string serverVariable in request.ServerVariables.AllKeys)
            {
                requestInfo.Append(delimiter);
                requestInfo.Append(serverVariable);
                requestInfo.Append("=");
                requestInfo.Append(request.ServerVariables[serverVariable]);
                delimiter = "\n";
            }
            loginAttempt.ServerVariables = requestInfo.ToString();
            loginAttempt.UserId = user.UserId;
            db.LoginAttempts.Add(loginAttempt);
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                logger.Error(exceptionMessage);
            }
        }
        #endregion
    }
}
