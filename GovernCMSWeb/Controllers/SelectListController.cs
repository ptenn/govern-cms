using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GovernCMS.Models;
using GovernCMS.Services;
using GovernCMS.Services.Impl;
using GovernCMS.Utils;
using GovernCMS.ViewModels;

namespace GovernCMS.Controllers
{
    public class SelectListController : ErrorHandlingController
    {
        private GovernCmsContext db = new GovernCmsContext();

        private readonly IWebsiteService websiteService;

        public SelectListController()
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

            foreach (var website in websites)
            {
                SelectListItem item = new SelectListItem()
                {
                    Text = website.SiteName,
                    Value = website.Id.ToString()
                };
                selectListItems.Add(item);
            }


            if (websites.Count > 0)
            {
                if (websiteId == null)
                {
                    websiteId = websites.First().Id;
                }
            }

            SelectListViewModel selectListViewModel = new SelectListViewModel()
            {
                WebsiteId = websiteId.GetValueOrDefault(),
                WebsiteSelectList = new SelectList(selectListItems, "Value", "Text")
            };
            return View(selectListViewModel);
        }
    }
}