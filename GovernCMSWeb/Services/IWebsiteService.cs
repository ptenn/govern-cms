using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GovernCMS.Models;

namespace GovernCMS.Services
{
    interface IWebsiteService
    {
        /// <summary>
        /// Create and persist a new Website.
        /// </summary>
        /// <param name="name">Website Name</param>
        /// <param name="url">Website URL</param>
        /// <param name="creator">Website Creator</param>
        /// <returns>Newly created Website</returns>
        Website CreateWebsite(string name, string url, User creator);
    }
}
