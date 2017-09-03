using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GovernCMS.Models;
using GovernCMS.Services;
using GovernCMS.Services.Impl;
using GovernCMS.Utils;
using GovernCMS.ViewModels;
using log4net;

namespace GovernCMS.Controllers
{
    public class ArtifactController : ErrorHandlingController
    {
        private static ILog logger = LogManager.GetLogger(typeof(UserController));

        private GovernCmsContext db = new GovernCmsContext();

        private IArtifactService artifactService = new ArtifactService();
        
        public ActionResult Index()
        {
            UserCheck();
            User currentUser = (User)Session[Constants.CURRENT_USER];

            IList<Artifact> artifacts = db.Artifacts.Where(a => a.OrganizationId == currentUser.OrganizationId).ToList();
            ArtifactListViewModel artifactListViewModel = new ArtifactListViewModel();
            artifactListViewModel.Artifacts = artifacts;
            return View(artifactListViewModel);
        }

        [HttpGet]
        public ActionResult Manage(int? artifactId)
        {
            ManageArtifactViewModel manageArtifactViewModel = new ManageArtifactViewModel();
            if (artifactId.HasValue)
            {
                Artifact artifact = artifactService.FindArtifactById(artifactId.Value, true);
                manageArtifactViewModel.ArtifactId = artifact.ArtifactId;
                manageArtifactViewModel.Name = artifact.Name;
                manageArtifactViewModel.Description = artifact.Description;
                manageArtifactViewModel.Version = artifact.Version;
                manageArtifactViewModel.OrganizationId = artifact.OrganizationId;
                manageArtifactViewModel.OwnerId = artifact.OwnerId;
            }
            return View("ManageArtifact", manageArtifactViewModel);
        }

        public ActionResult Delete(int artifactid)
        {
            throw new System.NotImplementedException();
        }

        [HttpPost]
        public JsonResult Upload(FormCollection formCollection, HttpPostedFileBase documentFile)
        {
            User currentUser = (User)Session[Constants.CURRENT_USER];

            logger.Info($"Form Collection Count: {formCollection.Count}");

            // Re-fetch the Artifact by ID
            Artifact artifact;
            int artifactId;
            if (formCollection["ArtifactIdUploadForm"] != null && int.TryParse(formCollection["ArtifactIdUploadForm"], out artifactId))
            {
                artifact = artifactService.FindArtifactById(artifactId, true);
            }
            else
            {
                string artifactName = formCollection["NameUploadForm"];
                string artifactDesc = formCollection["DescriptionUploadForm"];
                HttpPostedFileBase file = null;
                foreach (string fileName in Request.Files)
                {
                    file = Request.Files[fileName];
                }

                artifact = artifactService.CreateArtifactFromFile(artifactName, artifactDesc, file, currentUser);
            }

            return Json(artifact);
        }
    }
}