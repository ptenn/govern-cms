using System.Web.Mvc;

namespace GovernCMS.ViewModels
{
    public class CalendarViewModel
    {
        public int WebsiteId { get; set; }
        public SelectList WebsiteSelectList { get; set; }
    }
}