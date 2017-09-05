using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        public IList<Website> FindWebsitesByOrganizationId(int organizationId)
        {
            return db.Websites.Where(w => w.OrganizationId == organizationId).ToList();
        }

        public IList<Category> FindCategoriesByWebsiteId(int websiteId)
        {
            return db.Categories.Where(c => c.WebsiteId == websiteId).Include(c => c.SubCategories).ToList();
        }
    }
}