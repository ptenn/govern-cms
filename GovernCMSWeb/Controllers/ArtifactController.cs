using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        [HttpPost]
        public JsonResult Upload(FormCollection formCollection, HttpPostedFileBase documentFile)
        {
            User currentUser = (User)Session[Constants.CURRENT_USER];

            logger.Info($"Form Collection Count: {formCollection.Count}");

            // Re-fetch the Agenda by ID
            Agenda agenda = null;
            int agendaId;
            if (formCollection["AgendaId"] != null && int.TryParse(formCollection["AgendaId"], out agendaId))
            {
                agenda = db.Agenda.Find(agendaId);
            }

            // if the Agenda was not found, create a new one
            if (agenda == null)
            {
                agenda = new Agenda();
                agenda.OrganizationId = currentUser.OrganizationId.GetValueOrDefault();
                agenda.OwnerId = currentUser.UserId;
            }

            // Populate fields
            agenda.AgendaName = formCollection["AgendaName"];
            agenda.MeetingLocation = formCollection["MeetingLocation"];
            agenda.MeetingDateTime = StringUtils.ParseDateAndTime(formCollection["MeetingDate"], formCollection["MeetingTime"]);
            agenda.Type = formCollection["AgendaType"];
            agenda.TypeDesc = formCollection["AgendaTypeDesc"];
            agenda.AgendaOrigFileName = formCollection["AgendaOrigFileName"];


            CloudBlockBlob documentBlob;

            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];

                if (file != null && file.ContentLength != 0)
                {
                    documentBlob = BlobUtils.UploadAndSaveBlob(agendaBlobContainer, file);

                    // We will take the First File as the Agenda, and any subsequent ones will be assumed to be secondary attachments.
                    agenda.AgendaOrigUrl = documentBlob.Uri.ToString();
                    db.Agenda.AddOrUpdate(agenda);
                    db.SaveChanges();

                    BlobInformation blobInfo = new BlobInformation()
                    {
                        Id = agenda.AgendaId,
                        Type = IdType.Agenda,
                        BlobUri = new Uri(agenda.AgendaOrigUrl)
                    };
                    var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(blobInfo));
                    documentQueue.AddMessage(queueMessage);
                    logger.Debug(String.Format("Created queue message for Agenda ID {0}", agenda.AgendaId));
                }
            }
            ManageAgenda manageAgenda = new ManageAgenda();
            manageAgenda.AgendaId = agenda.AgendaId;
            manageAgenda.AgendaOrigFileName = agenda.AgendaOrigFileName;
            manageAgenda.AgendaPdfUrl = agenda.AgendaPdfUrl;
            return Json(manageAgenda);
        }
    }
}