using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using GovernCMS.Models;
using GovernCMS.Services;
using GovernCMS.Services.Impl;
using GovernCMS.Utils;
using GovernCMS.ViewModels;
using GovernCMS.Web;
using log4net;
using Newtonsoft.Json;

namespace GovernCMS.Controllers
{
    public class BreadcrumbController : ErrorHandlingController
    {
        private static ILog logger = LogManager.GetLogger(typeof(WebsiteController));

        private GovernCmsContext db = new GovernCmsContext();

        private readonly IWebsiteService websiteService;

        public BreadcrumbController()
        {
            websiteService = new WebsiteService();
        }

        [HttpGet]
        public ActionResult Manage(int? id)
        {
            UserCheck();
            User currentUser = (User) Session[Constants.CURRENT_USER];

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
                if (id == null)
                {
                    id = websites.First().Id;
                }
            }

            IList<Category> categories = websiteService.FindCategoriesByWebsiteId(id.Value);
            BreadcrumbViewModel breadcrumbViewModel = new BreadcrumbViewModel()
            {
                WebsiteId = id.Value,
                WebsiteSelectList = new SelectList(selectListItems, "Value", "Text"),
                Categories = categories
            };

            return View("ManageBreadcrumb", breadcrumbViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(BreadcrumbViewModel breadcrumbViewModel)
        {
            // Now, create all new Sections and Items, providing the Agenda Id for Referential Integrity
            IList<Category> categories =
                JsonConvert.DeserializeObject<IList<Category>>(breadcrumbViewModel.CategoriesJson);

            ProcessCategories(breadcrumbViewModel.WebsiteId, categories);

            db.SaveChanges();
            TempData["successMessage"] = "Breadcrumb Categories Saved";
            return RedirectToAction("Manage", new {websiteId = breadcrumbViewModel.WebsiteId});
        }

        private void ProcessCategories(int websiteId, IList<Category> categories)
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
                    foreach (Category subCategory in category.SubCategories)
                    {
                        if (category.CategoryId > 0)
                        {
                            subCategory.ParentCategoryId = category.CategoryId;
                        }
                        else
                        {
                            subCategory.ParentCategory = category;
                        }
                    }
                }

                // Recursive Call
                if (category.SubCategories != null && category.SubCategories.Count > 0)
                {
                    ProcessCategories(websiteId, category.SubCategories.ToList());
                }

                if (category.CategoryId > 0)
                {
                    db.Categories.Attach(category);
                    db.Entry(category).State = EntityState.Modified;
                }
                else
                {
                    db.Categories.Add(category);
                }
            }
        }

        // POST: Website/CategoryAdd
        [HttpPost]
        public JsonResult CategoryAdd(int websiteId, string categoryName)
        {
            UserCheck();

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

    }
}