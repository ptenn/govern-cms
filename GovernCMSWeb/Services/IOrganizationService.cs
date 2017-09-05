using System.Collections.Generic;
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

        /// <summary>
        /// Find all Websites for an Organization by Organization ID
        /// </summary>
        /// <param name="organizationId">The Organization ID for finding Websites</param>
        /// <returns>List of Websites for the Organization</returns>
        IList<Website> FindWebsitesByOrganizationId(int organizationId);
    }
}
