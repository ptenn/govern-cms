using System.Web.Mvc;
using GovernCMS.Models;
using GovernCMS.Services;
using GovernCMS.Services.Impl;
using GovernCMS.Utils;
using GovernCMS.ViewModels;
using log4net;

namespace GovernCMS.Controllers
{
    public class WebsiteController : Controller
    {
        private static ILog logger = LogManager.GetLogger(typeof(WebsiteController));

        private GovernCmsContext db = new GovernCmsContext();

        private readonly IOrganizationService organizationService;

        public WebsiteController()
        {
            organizationService = new OrganizationService();
        }

        [HttpGet]
        public ActionResult Create()
        {
            User currentUser = (User) Session[Constants.CURRENT_USER];

            ManageWebsiteViewModel manageWebsiteViewModel = new ManageWebsiteViewModel();
            manageWebsiteViewModel.Websites = organizationService.FindWebsitesByOrganizationId(currentUser.OrganizationId);
            manageWebsiteViewModel.OwnerId = currentUser.UserId;

            return View(manageWebsiteViewModel);
        }

        [HttpGet]
        public ActionResult Breadcrumb()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Calendar()
        {
            return View();
        }

        [HttpDelete]
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