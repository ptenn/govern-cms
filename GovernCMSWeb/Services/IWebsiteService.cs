using System.Collections.Generic;
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

        /// <summary>
        /// Find all Websites for an Organization.
        /// </summary>
        /// <param name="organizationId">The Organization ID for finding all Websites.</param>
        /// <returns>List of all Websites for Organization</returns>
        IList<Website> FindWebsitesByOrganizationId(int organizationId);

        /// <summary>
        /// Find all Websites for an Organization, including Boards
        /// </summary>
        /// <param name="organizationId">The Organization ID for finding all Websites.</param>
        /// <returns>List of all Websites for Organization, including Boards</returns>
        IList<Website> FindWebsitesByOrganizationIdIncludeBoards(int organizationId);

        /// <summary>
        /// Find Categories by Website, including Subcategories
        /// </summary>
        /// <param name="websiteId">The Website ID for finding all Categories</param>
        /// <returns>Categories, including subcategories</returns>
        IList<Category> FindCategoriesByWebsiteId(int websiteId);

        /// <summary>
        /// Find Calendars by Website, including Calendar Events
        /// </summary>
        /// <param name="websiteId">The Website ID for finding all Calendars</param>
        /// <returns>Calendars, including Events</returns>
        IList<Calendar> FindCalendarsByWebsiteId(int websiteId);

        /// <summary>
        /// Find Calendar Events by Calendar.
        /// </summary>
        /// <param name="calendarId">The Calendar ID</param>
        /// <returns>All Events for the Calendar</returns>
        IList<CalendarEvent> FindEventsByCalendarId(int calendarId);

        /// <summary>
        /// Finds all Boards by Website, including Board Cards
        /// </summary>
        /// <param name="websiteId">The Website ID for finding all Boards</param>
        /// <returns>Boards, including Cards</returns>
        IList<Board> FindBoardsByWebsiteId(int websiteId);
    }
}
