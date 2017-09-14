using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GovernCMS.Models;
using GovernCMS.Services;
using GovernCMS.Services.Impl;
using GovernCMS.Utils;
using GovernCMS.ViewModels;

namespace GovernCMS.Controllers
{
    public class CalendarController : ErrorHandlingController
    {
        private GovernCmsContext db = new GovernCmsContext();

        private readonly IWebsiteService websiteService;

        public CalendarController()
        {
            websiteService = new WebsiteService();
        }

        [HttpGet]
        public ActionResult Manage(int? websiteId)
        {
            UserCheck();
            User currentUser = (User)Session[Constants.CURRENT_USER];

            IList<Website> websites = websiteService.FindWebsitesByOrganizationId(currentUser.OrganizationId);
            IList<SelectListItem> selectListItems = new List<SelectListItem>();
            IList<SelectListItem> calendarSelectListItems = new List<SelectListItem>();


            IList<Calendar> allCalendars = new List<Calendar>();

            IDictionary<int, IList<Calendar>> websiteCalendars = new Dictionary<int, IList<Calendar>>();
            foreach (var website in websites)
            {
                SelectListItem item = new SelectListItem()
                {
                    Text = website.SiteName,
                    Value = website.Id.ToString()
                };
                selectListItems.Add(item);

                // Get the Calendars for each Website
                IList<Calendar> calendars = websiteService.FindCalendarsByWebsiteId(website.Id);

                if (calendars != null)
                {
                    foreach (var calendar in calendars)
                    {
                        allCalendars.Add(calendar);
                    }
                    websiteCalendars.Add(website.Id, calendars);
                }
            }

            int calendarId = 0;
            IList<CalendarEventViewModel> selectedCalendarEvents = new List<CalendarEventViewModel>();
            if (websites.Count > 0)
            {
                if (websiteId == null)
                {
                    websiteId = websites.First().Id;
                    if (websiteCalendars.ContainsKey(websiteId.Value))
                    {
                        Boolean firstCalendar = true;
                        IList<Calendar> selectedCalendars = websiteCalendars[websiteId.Value];
                        foreach (var calendar in selectedCalendars)
                        {
                            if (firstCalendar)
                            {
                                calendarId = calendar.CalendarId;
                                selectedCalendarEvents = calendar.CalendarEvents
                                    .Select(e => new CalendarEventViewModel()
                                    {
                                        Id = e.Id,
                                        CalendarId = e.CalendarId,
                                        EventName = e.EventName,
                                        EventUrl = e.EventUrl,
                                        StartDate = e.StartDate.ToString("yyyy-MM-dd"),
                                        EndDate = e.EndDate.ToString("yyyy-MM-dd")
                                    })
                                    .ToList();
                                firstCalendar = false;
                            }
                            SelectListItem item = new SelectListItem()
                            {
                                Text = calendar.CalendarName,
                                Value = calendar.CalendarId.ToString()
                            };
                            calendarSelectListItems.Add(item);
                        }
                    }

                }
            }

            CalendarViewModel calendarViewModel = new CalendarViewModel()
            {
                WebsiteId = websiteId.GetValueOrDefault(),
                CalendarId = calendarId,
                WebsiteSelectList = new SelectList(selectListItems, "Value", "Text"),
                CalendarSelectList = new SelectList(calendarSelectListItems, "Value", "Text"),
                SelectedCalendarEvents = selectedCalendarEvents
            };

            return View("ManageCalendar", calendarViewModel);
        }

        [HttpPost]
        public JsonResult CalendarAdd(int websiteId, string calendarName)
        {
            Calendar calendar = new Calendar();
            calendar.CalendarName = calendarName;
            calendar.CreateDate = DateTime.Now.Date;
            calendar.WebsiteId = websiteId;
            db.Calendars.Add(calendar);
            db.SaveChanges();

            TempData["successMessage"] = "Calendar " + calendarName + " saved";

            return Json(calendar);
        }

        [HttpPost]
        public JsonResult CalendarEventAdd(int calendarId, string eventName, string startDate, string endDate, string eventUrl)
        {
            CalendarEvent calendarEvent = new CalendarEvent();
            calendarEvent.EventName = eventName;
            calendarEvent.StartDate = DateTime.Parse(startDate);
            calendarEvent.EndDate = DateTime.Parse(endDate);
            calendarEvent.CreateDate = DateTime.Now.Date;
            calendarEvent.EventUrl = eventUrl;
            calendarEvent.CalendarId = calendarId;
            db.CalendarEvents.Add(calendarEvent);
            db.SaveChanges();

            CalendarEventViewModel calendarEventViewModel = new CalendarEventViewModel()
            {
                Id = calendarEvent.Id,
                CalendarId = calendarEvent.CalendarId,
                EventName = calendarEvent.EventName,
                StartDate = calendarEvent.StartDate.ToString("yyyy-MM-dd"),
                EndDate = calendarEvent.EndDate.ToString("yyyy-MM-dd"),
                EventUrl = calendarEvent.EventUrl
            };

            return Json(calendarEventViewModel);
        }

        [HttpPost]
        public JsonResult FindEventsByCalendarId(int calendarId)
        {
            IList<CalendarEventViewModel> events = websiteService.FindEventsByCalendarId(calendarId)
                .Select(e => new CalendarEventViewModel()
                {
                    Id = e.Id,
                    CalendarId = e.CalendarId,
                    EventName = e.EventName,
                    EventUrl = e.EventUrl,
                    StartDate = e.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = e.EndDate.ToString("yyyy-MM-dd")
                })
                .ToList();

            return Json(events);
        }
    }
}