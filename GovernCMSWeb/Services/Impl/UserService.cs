using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Security;
using GovernCMS.Models;
using GovernCMS.Utils;

namespace GovernCMS.Services.Impl
{
    public class UserService : IUserService
    {
        private GovernCmsContext db = new GovernCmsContext();

        public User CreateUser(string emailAddr, string passwd, string confirmPasswd, string firstName, string lastName,
                               int?  organizationId, string organizationName)
        {
            // Check to see if there is already a User registered with this email address
            string cleanEmailAddr = StringUtils.CleanEmailAddr(emailAddr);
            User existingUserCheck = db.Users.FirstOrDefault(u => u.EmailAddr == cleanEmailAddr);
            if (existingUserCheck != null)
            {
                throw new MembershipCreateUserException("There is already a User Registered with this Email Address.");
            }

            // No existing User registered with Email Address - proceed

            // Sanitize the Email Address
            emailAddr = StringUtils.CleanEmailAddr(emailAddr);

            // At this point, we should be good
            DateTime currentDate = DateTime.Now.Date;
            User user = new User();
            user.CreateDate = currentDate;
            user.UpdateDate = currentDate;
            user.EmailAddr = emailAddr;

            // Get Password Salt
            Random random = new Random();
            int salt = random.Next();
            user.Salt = salt;

            string hashedPasswd = PasswordUtils.Sha256(passwd + salt);

            // Store encrypted Password
            user.Passwd = hashedPasswd;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Admin = false;

            // Set User as Company Admin
            user.Type = Constants.USER_TYPE_ADMINISTRATOR;

            // Existing Organization
            if (organizationId.HasValue)
            {
                user.OrganizationId = organizationId.Value;
            }
            // Potentially new
            else
            {
                // Try to lookup Organization by Slug
                string slug = StringUtils.CreateSlug(organizationName);
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
                    organization.Name = organizationName;
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

            return user;
        }

        public User EditUser(int userId, string passwd, string confirmPasswd, string firstName, string lastName)
        {
            User user = db.Users.Find(userId);
            // Guard block
            if (user == null)
            {
                throw new MembershipCreateUserException("Unable to find User");
            }

            if (!string.IsNullOrEmpty(passwd) &&
                !string.IsNullOrEmpty(confirmPasswd) &&
                passwd.Equals(confirmPasswd))
            {
                string hashedPasswd = PasswordUtils.Sha256(passwd + user.Salt);
                user.Passwd = hashedPasswd;
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                user.FirstName = firstName;
            }
            if (!string.IsNullOrEmpty(lastName))
            {
                user.LastName = lastName;
            }

            user.UpdateDate = DateTime.Now.Date;

            db.Users.AddOrUpdate(user);
            db.SaveChanges();

            return user;
        }
    }
}