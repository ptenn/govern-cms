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
using Login = System.Web.UI.WebControls.Login;

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
        public ActionResult Create(
            [Bind(Include = "EmailAddr, Passwd, ConfirmPasswd, FirstName,LastName")] CreateUser createUser)
        {
            if (ModelState.IsValid)
            {
                // Check to see if there is already a User registered with this email address
                string cleanEmailAddr = StringUtils.CleanEmailAddr(createUser.EmailAddr);
                User existingUserCheck = db.Users.FirstOrDefault(u => u.EmailAddr == cleanEmailAddr);
                if (existingUserCheck != null)
                {
                    ModelState.AddModelError("ErrorMessage",
                        "There is already a User Registered with this Email Address.");
                    return View(createUser);
                }

                // No existing User registered with Email Address - proceed

                // Sanitize the Email Address
                createUser.EmailAddr = StringUtils.CleanEmailAddr(createUser.EmailAddr);

                // At this point, we should be good
                User user = new User();
                user.UpdateDate = DateTime.Now;
                user.EmailAddr = createUser.EmailAddr;

                // Get Password Salt
                Random random = new Random();
                int salt = random.Next();
                user.Salt = salt;

                string hashedPasswd = PasswordUtils.Sha256(createUser.Passwd + salt);

                // Store encrypted Password
                user.Passwd = hashedPasswd;
                user.FirstName = createUser.FirstName;
                user.LastName = createUser.LastName;
                user.CreateDate = DateTime.Now;
                user.Admin = false;

                // Set User as Company Admin
                user.Type = Constants.USER_TYPE_ADMINISTRATOR;

                db.Users.Add(user);
                db.SaveChanges();

                // Put User in the session
                Session[Constants.CURRENT_USER] = user;

                RecordLoginAttempt(user, Request);

                logger.Debug("Created User " + user.UserId);

                return RedirectToAction("ManageOrganization", "Organization");
            }

            return View(createUser);
        }

        // Get: User/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login login)
        {
            // First, find the Real User based on the email address
            string cleanEmailAddr = StringUtils.CleanEmailAddr(login.EmailAddr);

            // There should only be one result due to AK on CleanEmailAddr column in DB
            User user = db.Users
                .Include(u => u.Organization)
                .FirstOrDefault(u => u.EmailAddr == cleanEmailAddr);

            if (user == null)
            {
                ModelState.AddModelError("ErrorMessage", "Email Address or Password was invalid");
                return View();
            }

            var encrypedPass = PasswordUtils.Sha256(login.Passwd + user.Salt);

            // user entered correct password
            if (encrypedPass.Equals(user.Passwd))
            {
                // Login successful, Put User in the session
                Session[Constants.CURRENT_USER] = user;

                // Record Login Attempt
                RecordLoginAttempt(user, Request);

                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ModelState.AddModelError("ErrorMessage", "Email Address or Password was invalid");
                return View();
            }
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
                CreateUser createUserVM = new CreateUser();
                createUserVM.UserId = user.UserId;
                createUserVM.EmailAddr = user.EmailAddr;
                createUserVM.FirstName = user.FirstName;
                createUserVM.LastName = user.LastName;

                return View(createUserVM);
            }
            else
            {
                TempData["errorMessage"] = "Unable to Edit User";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(CreateUser createUser)
        {
            User user = db.Users.Find(createUser.UserId);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(createUser.Passwd) &&
                    !string.IsNullOrEmpty(createUser.ConfirmPasswd) &&
                    createUser.Passwd.Equals(createUser.ConfirmPasswd))
                {
                    string hashedPasswd = PasswordUtils.Sha256(createUser.Passwd + user.Salt);
                    user.Passwd = hashedPasswd;
                }

                if (!string.IsNullOrEmpty(createUser.FirstName))
                {
                    user.FirstName = createUser.FirstName;
                }
                if (!string.IsNullOrEmpty(createUser.LastName))
                {
                    user.LastName = createUser.LastName;
                }

                db.Users.AddOrUpdate(user);
                db.SaveChanges();
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
            if (!currentUser.OrganizationId.HasValue)
            {
                throw new OrganizationException("You must belong to an Organization to create an Agenda. Please provide Organization Information.");
            }

            IList<User> users;
            if (string.IsNullOrEmpty(term))
            {
                return null;
            }
            if (term.Contains(' '))
            {
                string[] sTokens = term.Split(' ');
                users = db.Users.Where(u => u.OrganizationId == currentUser.OrganizationId.Value && 
                                        u.FirstName.ToLower().StartsWith(sTokens[0].ToLower()) || 
                                        u.LastName.ToLower().StartsWith(sTokens[1].ToLower())).ToList();
            }
            else
            {
                users = db.Users.Where(u => u.OrganizationId == currentUser.OrganizationId.Value && 
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

        #region Helper Methods
        private void RecordLoginAttempt(IUser user, HttpRequestBase request)
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
