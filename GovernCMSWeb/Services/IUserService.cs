using System.Collections.Generic;
using GovernCMS.Models;

namespace GovernCMS.Services
{
    interface IUserService
    {
        /// <summary>
        /// Find a User by unique ID
        /// </summary>
        /// <param name="userId">ID of User to Find</param>
        /// <returns>User with unique ID, null if not found</returns>
        User FindUserById(int userId);

        /// <summary>
        /// Create a New User, persist and return User
        /// </summary>
        /// <param name="emailAddr">User Email Address</param>
        /// <param name="passwd">User Password</param>
        /// <param name="confirmPasswd">Confirm user Password</param>
        /// <param name="firstName">First Name</param>
        /// <param name="lastName">Last Name</param>
        /// <param name="organizationId">Optional Organization ID</param>
        /// <param name="organizationName">Optional Organization Name</param>
        /// <returns>Newly-created User</returns>
        User CreateUser(string emailAddr, string passwd, string confirmPasswd, string firstName, string lastName, 
            int? organizationId, string organizationName);

        /// <summary>
        /// Update an existing User.  Any null/blank values are not updated.
        /// </summary>
        /// <param name="userId">ID of User to Update</param>
        /// <param name="passwd">New User Password</param>
        /// <param name="confirmPasswd">Confirm user Password</param>
        /// <param name="firstName">New First Name</param>
        /// <param name="lastName">New Last Name</param>
        /// <returns>Updated User</returns>
        User EditUser(int userId, string passwd, string confirmPasswd, string firstName, string lastName);

        /// <summary>
        /// Find Users by search term
        /// </summary>
        /// <param name="term">Search Term</param>
        /// <param name="organizationId">The Organization ID</param>
        /// <returns>List of matching Users</returns>
        IList<User> FindUsers(string term, int organizationId);
    }
}
