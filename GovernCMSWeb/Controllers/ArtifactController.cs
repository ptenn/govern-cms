using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GovernCMS.Models;
using GovernCMS.Services;
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
            ArtifactList artifactList = new ArtifactList();
            artifactList.Artifacts = artifacts;
            return View(artifactList);
        }

        [HttpGet]
        public ActionResult ManageArtifact(int? artifactId)
        {
            ManageArtifact manageArtifact = new ManageArtifact();
            if (artifactId.HasValue)
            {
                Artifact artifact = artifactService.FindArtifactById(artifactId.Value, true);
                manageArtifact.ArtifactId = artifact.ArtifactId;
                manageArtifact.Name = artifact.Name;
                manageArtifact.Description = artifact.Description;
                manageArtifact.Version = artifact.Version;
                manageArtifact.OrganizationId = artifact.OrganizationId;
                manageArtifact.OwnerId = artifact.OwnerId;
            }
            return View();
        }

        public ActionResult Manage(int artifactid)
        {
            throw new System.NotImplementedException();
        }

        public ActionResult Delete(int artifactid)
        {
            throw new System.NotImplementedException();
        }
    }
}