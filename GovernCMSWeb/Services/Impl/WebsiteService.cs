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

        public IList<Website> FindWebsitesByOrganizationIdIncludeBoards(int organizationId)
        {
            return db.Websites.Where(w => w.OrganizationId == organizationId).Include(w => w.Boards).ToList();
        }

        public IList<Category> FindCategoriesByWebsiteId(int websiteId)
        {
            var categories = db.Categories
                .Where(c => c.WebsiteId == websiteId)
                .Where(c => c.ParentCategoryId == null) // filter sub-categories
                .Include(c => c.SubCategories)
                .ToList();
            return categories;
        }

        public IList<Calendar> FindCalendarsByWebsiteId(int websiteId)
        {
            IList<Calendar> calendars = db.Calendars.Where(c => c.WebsiteId == websiteId)
                                                    .Include(c => c.CalendarEvents)
                                                    .OrderBy(c => c.CalendarId)
                                                    .ToList();
            return calendars;
        }

        public IList<CalendarEvent> FindEventsByCalendarId(int calendarId)
        {
            IList<CalendarEvent> events = db.CalendarEvents.Where(e => e.CalendarId == calendarId)
                                                           .OrderBy(e => e.Id)
                                                           .ToList();
            return events;
        }

        public IList<Board> FindBoardsByWebsiteId(int websiteId)
        {
            IList<Board> boards = db.Boards.Where(b => b.WebsiteId == websiteId)
                                           .Include(b => b.BoardCards)
                                           .ToList();
            return boards;
        }


    }
}