using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GovernCMS.Models;
using GovernCMS.Utils;
using GovernCMS.ViewModels;
using log4net;

namespace GovernCMS.Controllers
{
    public class UserController : ErrorHandlingController
    {
        private static ILog logger = LogManager.GetLogger(typeof(UserController));

        private GovernCmsContext db = new GovernCmsContext();

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
                // Check to see if there is already a User registered with this email address
                string cleanEmailAddr = StringUtils.CleanEmailAddr(createUserViewModel.EmailAddr);
                User existingUserCheck = db.Users.FirstOrDefault(u => u.EmailAddr == cleanEmailAddr);
                if (existingUserCheck != null)
                {
                    ModelState.AddModelError("ErrorMessage",
                        "There is already a User Registered with this Email Address.");
                    return View(createUserViewModel);
                }

                // No existing User registered with Email Address - proceed

                // Sanitize the Email Address
                createUserViewModel.EmailAddr = StringUtils.CleanEmailAddr(createUserViewModel.EmailAddr);

                // At this point, we should be good
                User user = new User();
                user.UpdateDate = DateTime.Now;
                user.EmailAddr = createUserViewModel.EmailAddr;

                // Get Password Salt
                Random random = new Random();
                int salt = random.Next();
                user.Salt = salt;

                string hashedPasswd = PasswordUtils.Sha256(createUserViewModel.Passwd + salt);

                // Store encrypted Password
                user.Passwd = hashedPasswd;
                user.FirstName = createUserViewModel.FirstName;
                user.LastName = createUserViewModel.LastName;
                user.CreateDate = DateTime.Now;
                user.Admin = false;

                // Set User as Company Admin
                user.Type = Constants.USER_TYPE_ADMINISTRATOR;

                // Existing Organization
                if (createUserViewModel.OrganizationId.HasValue)
                {
                    user.OrganizationId = createUserViewModel.OrganizationId.Value;
                }
                // Potentially new
                else
                { 
                    // Try to lookup Organization by Slug
                    string slug = StringUtils.CreateSlug(createUserViewModel.OrganizationName);
                    Organization organization = db.Organizations.FirstOrDefault(o => o.Slug.Equals(slug));

                    // existing organization found by Slug
                    if (organization != null)
                    {
                        user.OrganizationId = organization.OrganizationId;
                    }
                    // Completely new Organization
                    else
                    {
                        organization = new Organization();
                        organization.Name = createUserViewModel.OrganizationName;
                        organization.Slug = slug;
                        organization.CreateDate = DateTime.Now.Date;

                        // Attempt to get Email Host
                        string domain = EmailUtils.GetDomainFromEmailAddr(user.EmailAddr);
                        if (!string.IsNullOrEmpty(domain) && !EmailUtils.IsEmailHostCommonProvider(domain))
                        {
                            organization.EmailHost = domain.Trim().ToLower();
                        }
                        user.Organization = organization;
                        db.Organizations.Add(organization);
                    }
                }
                
                db.Users.Add(user);
                db.SaveChanges();

                // Put User in the session
                Session[Constants.CURRENT_USER] = user;

                // Record LoginViewModel Attempt
                RecordLoginAttempt(user, Request);

                logger.Debug("Created User " + user.UserId);
                TempData["successMessage"] = $"User {user.FirstName} {user.LastName} successfully created";

                return RedirectToAction("Index", "Home");
            }

            return View(createUserViewModel);
        }

        // Get: User/LoginViewModel
        public ActionResult Login()
        {
            return View();
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
            User currentUser = (User) Session[Constants.CURRENT_USER];

            // Guard block
            if (currentUser == null)
            {
                throw new AuthenticationException(
                    "You must be logged in and have an active Session in order to Edit Profile.");
            }

            User user = db.Users.Find(currentUser.UserId);

            if (user != null)
            {
                CreateUserViewModel createUserViewModel = new CreateUserViewModel();
                createUserViewModel.UserId = user.UserId;
                createUserViewModel.EmailAddr = user.EmailAddr;
                createUserViewModel.FirstName = user.FirstName;
                createUserViewModel.LastName = user.LastName;

                return View(createUserViewModel);
            }
            else
            {
                TempData["errorMessage"] = "Unable to Edit User";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(CreateUserViewModel createUserViewModel)
        {
            User user = db.Users.Find(createUserViewModel.UserId);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(createUserViewModel.Passwd) &&
                    !string.IsNullOrEmpty(createUserViewModel.ConfirmPasswd) &&
                    createUserViewModel.Passwd.Equals(createUserViewModel.ConfirmPasswd))
                {
                    string hashedPasswd = PasswordUtils.Sha256(createUserViewModel.Passwd + user.Salt);
                    user.Passwd = hashedPasswd;
                }

                if (!string.IsNullOrEmpty(createUserViewModel.FirstName))
                {
                    user.FirstName = createUserViewModel.FirstName;
                }
                if (!string.IsNullOrEmpty(createUserViewModel.LastName))
                {
                    user.LastName = createUserViewModel.LastName;
                }

                db.Users.AddOrUpdate(user);
                db.SaveChanges();

                // Put Updated User in the session
                Session[Constants.CURRENT_USER] = user;

                TempData["successMessage"] = "User Successfully Updated";
            }
            else
            {
                TempData["errorMessage"] = "Unable to update User";
            }
            
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public JsonResult FindUsers(string term)
        {
            User currentUser = (User)Session[Constants.CURRENT_USER];
            // Guard blocks
            if (currentUser == null)
            {
                throw new AuthenticationException("You must be logged in to create an Agenda.");
            }
            IList<User> users;
            if (string.IsNullOrEmpty(term))
            {
                return null;
            }
            if (term.Contains(' '))
            {
                string[] sTokens = term.Split(' ');
                users = db.Users.Where(u => u.OrganizationId == currentUser.OrganizationId && 
                                        u.FirstName.ToLower().StartsWith(sTokens[0].ToLower()) || 
                                        u.LastName.ToLower().StartsWith(sTokens[1].ToLower())).ToList();
            }
            else
            {
                users = db.Users.Where(u => u.OrganizationId == currentUser.OrganizationId && 
                                            u.FirstName.ToLower().StartsWith(term.ToLower()) ||
                                            u.LastName.ToLower().StartsWith(term.ToLower())).ToList();
            }

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
            Organization organization = null;

            // Guard block
            if (string.IsNullOrEmpty(emailAddr))
            {
                searchResultViewModel.Match = false;
                searchResultViewModel.AllOrgNames = db.Organizations.Select(o => o.Name).ToList();
                return Json(searchResultViewModel);
            }
            
            string domain = EmailUtils.GetDomainFromEmailAddr(emailAddr).Trim().ToLower();
            if (!string.IsNullOrEmpty(domain) && !EmailUtils.IsEmailHostCommonProvider(domain))
            {
                organization = db.Organizations.FirstOrDefault(o => o.EmailHost.Equals(domain));
                if (organization != null)
                {
                    searchResultViewModel.Match = true;
                    searchResultViewModel.OrgId = organization.OrganizationId;
                    searchResultViewModel.OrgName = organization.Name;
                }
            }

            if (organization == null)
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
