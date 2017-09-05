using System;
using GovernCMS.Models;

namespace GovernCMS.Services.Impl
{
    public class WebsiteService : IWebsiteService
    {
        private GovernCmsContext db = new GovernCmsContext();

        public Website CreateWebsite(string name, string url, User creator)
        {
            DateTime currentDate = DateTime.Now.Date;
            Website website = new Website()
            {
                SiteName = name,
                SiteUrl = url,
                OwnerId = creator.UserId,
                OrganizationId = creator.OrganizationId,
                CreateDate = currentDate,
                UpdateDate = currentDate
            };

            db.Websites.Add(website);
            db.SaveChanges();
            return website;
        }
    }
}