using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GovernCMS.Models;
using GovernCMS.Services;
using GovernCMS.Services.Impl;
using GovernCMS.Utils;
using GovernCMS.ViewModels;
using log4net;
using Newtonsoft.Json;

namespace GovernCMS.Controllers
{
    public class WebsiteController : ErrorHandlingController
    {
        private static ILog logger = LogManager.GetLogger(typeof(WebsiteController));

        private GovernCmsContext db = new GovernCmsContext();

        private readonly IOrganizationService organizationService;

        private readonly IWebsiteService websiteService;

        public WebsiteController()
        {
            organizationService = new OrganizationService();
            websiteService = new WebsiteService();
        }

        [HttpGet]
        public ActionResult Create()
        {
            UserCheck();
            User currentUser = (User) Session[Constants.CURRENT_USER];

            ManageWebsiteViewModel manageWebsiteViewModel = new ManageWebsiteViewModel();
            manageWebsiteViewModel.Websites = organizationService.FindWebsitesByOrganizationId(currentUser.OrganizationId);
            manageWebsiteViewModel.OwnerId = currentUser.UserId;

            return View(manageWebsiteViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ManageWebsiteViewModel manageWebsiteViewModel)
        {
            UserCheck();
            User currentUser = (User)Session[Constants.CURRENT_USER];

            websiteService.CreateWebsite(manageWebsiteViewModel.SiteName, manageWebsiteViewModel.SiteUrl, currentUser);

            return RedirectToAction("Create");
        }

        [HttpGet]
        public ActionResult Breadcrumb(int? websiteId)
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
            IList<Category> categories = websiteService.FindCategoriesByWebsiteId(websiteId.Value);
            BreadcrumbViewModel breadcrumbViewModel = new BreadcrumbViewModel()
            {
                WebsiteId = websiteId.Value,
                WebsiteSelectList = new SelectList(selectListItems, "Value", "Text"),
                Categories = categories
            };
            
            return View(breadcrumbViewModel);
        }

        // POST: Website/CategoryAdd
        [HttpPost]
        public JsonResult CategoryAdd(int websiteId, string categoryName)
        {
            UserCheck();
            User currentUser = (User)Session[Constants.CURRENT_USER];

            // Delete and replace.
            Category category = new Category();
            category.WebsiteId = websiteId;
            category.CategoryName = categoryName;
            category.CreateDate = DateTime.Now.Date;
            category.Number = 0;

            // Get the highest Category Number
            List<Category> categories;
            categories = db.Categories.Where(c => c.WebsiteId == websiteId).OrderByDescending(c => c.Number).ToList();
            
            if (categories.Count > 0)
            {
                Category topCategory = categories.First();
                if (topCategory != null)
                {
                    category.Number = topCategory.Number + 1;
                }
            }

            db.Categories.Add(category);
            db.SaveChanges();

            return Json(category);
        }

        // POST: Website/CategoryDelete
        [HttpPost]
        public JsonResult CategoryDelete(int categoryId)
        {
            // Delete and replace.
            var deleteEntries = db.Categories.Where(c => c.CategoryId == categoryId);
            foreach (var entry in deleteEntries)
            {
                db.Categories.Remove(entry);
            }
            db.SaveChanges();
            return Json(categoryId);
        }

        // POST: Website/CategoryDeleteAll
        [HttpPost]
        public void CategoryDeleteAll(int websiteId)
        {
            // Delete and replace.
            var deleteEntries = db.Categories.Where(c => c.WebsiteId == websiteId);
            foreach (var entry in deleteEntries)
            {
                db.Categories.Remove(entry);
            }
            db.SaveChanges();
        }

        [HttpPost]
        public ActionResult SaveCategories(int websiteId, IList<Category> categories)
        {
            // Now, create all new Sections and Items, providing the Agenda Id for Referential Integrity
//            IList<Category> categories = JsonConvert.DeserializeObject<IList<Category>>(categoriesAsJson);

            categories = ProcessCategories(websiteId, categories);

            db.SaveChanges();
            return RedirectToAction("Breadcrumb", new { websiteId = websiteId});
        }

        private IList<Category> ProcessCategories(int websiteId, IList<Category> categories)
        {
            int number = 0;
            foreach (Category category in categories)
            {
                category.WebsiteId = websiteId;
                category.CreateDate = DateTime.Now.Date;
                category.Number = number;
                number++;
                if (category.SubCategories != null && category.SubCategories.Count > 0)
                {
                    ProcessCategories(websiteId, category.SubCategories.ToList());
                }
            }
            db.Categories.AddRange(categories);
            
            return categories;
        }


        [HttpGet]
        public ActionResult Calendar()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Delete(int siteId)
        {
            Website siteToDelete = new Website() { Id = siteId};
            db.Websites.Attach(siteToDelete);
            db.Websites.Remove(siteToDelete);
            db.SaveChanges();

            return Json(siteId);
        }
    }
}