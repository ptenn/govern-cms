namespace GovernCMS.ViewModels
{
    public class CalendarEventViewModel
    {
        public int Id { get; set; }
        public string EventName { get; set; }        
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string EventUrl { get; set; }
        public int CalendarId { get; set; }
    }
}