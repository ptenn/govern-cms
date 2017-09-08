using System.Collections.Generic;
using System.Web.Mvc;
using GovernCMS.Models;

namespace GovernCMS.ViewModels
{
    public class CalendarViewModel
    {
        public int WebsiteId { get; set; }
        public SelectList WebsiteSelectList { get; set; }

        public int CalendarId { get; set; }
        public IList<Calendar> Calendars { get; set; }
        public SelectList CalendarSelectList { get; set; }
    }
}