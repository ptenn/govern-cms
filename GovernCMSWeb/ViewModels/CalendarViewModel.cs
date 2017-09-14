using System.Collections.Generic;
using System.Web.Mvc;
using GovernCMS.Models;

namespace GovernCMS.ViewModels
{
    public class CalendarViewModel : WebsiteComponentViewModel
    {
        public int CalendarId { get; set; }
        public IList<Calendar> Calendars { get; set; }
        public SelectList CalendarSelectList { get; set; }

        public IList<CalendarEventViewModel> SelectedCalendarEvents { get; set; }
    }
}