using System.Linq;
using GovernCMS.Models;
using GovernCMS.Utils;

namespace GovernCMS.Services.Impl
{
    public class OrganizationService : IOrganizationService
    {
        private GovernCmsContext db = new GovernCmsContext();

        public Organization FindOrganizationByEmailAddr(string emailAddr)
        {
            Organization organization = null;

            // Guard block
            if (!string.IsNullOrEmpty(emailAddr))
            {
                string domain = EmailUtils.GetDomainFromEmailAddr(emailAddr).Trim().ToLower();
                if (!string.IsNullOrEmpty(domain) && !EmailUtils.IsEmailHostCommonProvider(domain))
                {
                    organization = db.Organizations.FirstOrDefault(o => o.EmailHost.Equals(domain));

                }
            }
            return organization;
        }
    }
}