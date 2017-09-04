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

        /// <summary>
        /// Entry point for ManageArtifact Page
        /// </summary>
        /// <param name="artifactId">Optional Artifact ID, toggles between Create and Update modes</param>
        /// <returns>View with ViewModel for populating page</returns>
        [HttpGet]
        public ActionResult Manage(int? artifactId)
        {            
            Artifact artifact = null;
            if (artifactId.HasValue)
            {
                artifact = artifactService.FindArtifactById(artifactId.Value, true);
            }
            ManageArtifactViewModel manageArtifactViewModel = ConvertArtifactToViewModel(artifact);
            return View("ManageArtifact", manageArtifactViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(ManageArtifactViewModel manageArtifactViewModel)
        {
            UserCheck();
            User currentUser = (User)Session[Constants.CURRENT_USER];
            HttpPostedFileBase file = null;
            foreach (string fileName in Request.Files)
            {
                file = Request.Files[fileName];
            }

            Artifact artifact;
            if (manageArtifactViewModel.ArtifactId != null)
            {
                artifact = artifactService.FindArtifactById(manageArtifactViewModel.ArtifactId.Value, false);
                artifact = artifactService.AddContentToArtifact(artifact, file, manageArtifactViewModel.ContentHtml,
                    currentUser);
            }
            else
            {
                if (file == null)
                {
                    artifact = artifactService.CreateArtifactFromContent(manageArtifactViewModel.Name,
                        manageArtifactViewModel.Description, manageArtifactViewModel.ContentHtml, currentUser);
                }
                else
                {
                    artifact = artifactService.CreateArtifactFromFile(manageArtifactViewModel.Name,
                        manageArtifactViewModel.Description, file, currentUser);
                }                
            }
            // Model has been updated, update ViewModel.
            manageArtifactViewModel = ConvertArtifactToViewModel(artifact);
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

        /// <summary>
        /// Helper method, converts Artifact Model to ViewModel.
        /// </summary>
        /// <param name="artifact">The Artifact Entity Model</param>
        /// <returns>Manage Artifact ViewModel</returns>
        private ManageArtifactViewModel ConvertArtifactToViewModel(Artifact artifact)
        {
            if (artifact == null)
            {
                return new ManageArtifactViewModel();
            }
            ManageArtifactViewModel manageArtifactViewModel = new ManageArtifactViewModel()
            {
                ArtifactId = artifact.ArtifactId,
                Name = artifact.Name,
                Description = artifact.Description,
                Version = artifact.Version,
                OrganizationId = artifact.OrganizationId,
                OwnerId = artifact.OwnerId,
                ContentItems = artifact.Contents.ToList()
            };
            return manageArtifactViewModel;
        }
    }
}