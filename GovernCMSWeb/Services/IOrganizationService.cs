using GovernCMS.Models;

namespace GovernCMS.Services
{
    interface IOrganizationService
    {
        /// <summary>
        /// Find an Organization by Email Address
        /// </summary>
        /// <param name="emailAddr">Email Address that belongs to the Organization</param>
        /// <returns>Organization that owns the Email Address</returns>
        Organization FindOrganizationByEmailAddr(string emailAddr);
    }
}
