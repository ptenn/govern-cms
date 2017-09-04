using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using GovernCMS.Azure;
using GovernCMS.Models;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GovernCMS.Services.Impl
{
    public class ArtifactService : IArtifactService
    {
        private GovernCmsContext db = new GovernCmsContext();

        private static CloudBlobContainer cmsContentBlobContainer;

        public ArtifactService()
        {
            
        }

        public Artifact CreateArtifactFromFile(string artifactName, string description, HttpPostedFileBase contentFile, 
            DateTime publishDate, User creator)
        {
            // Store File and get URL
            Artifact artifact = CreateArtifact(artifactName, description, contentFile, null, publishDate, creator);
            return artifact;
        }

        public Artifact CreateArtifactFromContent(string artifactName, string description, string content,
            DateTime publishDate, User creator)
        {
            Artifact artifact = CreateArtifact(artifactName, description, null, content, publishDate, creator);
            return artifact;
        }

        public IList<Artifact> FindArtifactsByUser(User user)
        {
            IList<Artifact> artifacts = new List<Artifact>();

            if (user != null && user.UserId > 0)
            {
                artifacts = db.Artifacts.Where(a => a.OwnerId == user.UserId).ToList();
            }
            return artifacts;
        }

        public IList<Artifact> FindArtifactsByGroup(Group group)
        {
            IList<Artifact> artifacts = new List<Artifact>();

            if (group != null && group.Id > 0)
            {
                artifacts = db.Artifacts.Where(a => a.Groups.All(g => g.Id == group.Id)).ToList();
            }
            return artifacts;
        }

        public Artifact FindArtifactById(int id, bool includeContent)
        {
            if (includeContent)
            {
                return db.Artifacts.Where(a => a.ArtifactId == id).Include(a => a.Contents).Include(a => a.User).FirstOrDefault();
            }
            return db.Artifacts.Where(a => a.ArtifactId == id).Include(a => a.User).FirstOrDefault();
        }

        public Artifact UpdateArtifact(Artifact artifact)
        {
            db.Artifacts.Attach(artifact);
            db.Entry(artifact).State = EntityState.Modified;
            db.SaveChanges();
            return artifact;
        }

        public Artifact AddContentToArtifact(Artifact artifact, HttpPostedFileBase contentFile, string contentHtml, 
            DateTime publishDate, User creator)
        {
            Content content = CreateContent(contentFile, artifact, publishDate, creator);
            db.Contents.Add(content);

            artifact.Contents.Add(content);
            db.Artifacts.Attach(artifact);
            db.Entry(artifact).State = EntityState.Modified;

            db.SaveChanges();
            

            return artifact;
        }

        private Artifact CreateArtifact(string artifactName, string description, HttpPostedFileBase contentFile, string contentHtml,
            DateTime publishDate, User creator)
        {
            DateTime currentDateTime = DateTime.Now.Date;
            Artifact artifact = new Artifact();
            artifact.Name = artifactName;
            artifact.Description = description;
            artifact.CreateDate = currentDateTime;
            artifact.OwnerId = creator.UserId;
            artifact.Version = 0;
            artifact.OrganizationId = creator.OrganizationId;
            artifact.Contents = new List<Content>();
            
            if (contentFile == null && !string.IsNullOrEmpty(contentHtml))
            {
                Content content = new Content();
                content.Artifact = artifact;
                content.ContentHtml = contentHtml;                
                content.CreateDate = currentDateTime;
                content.UpdateDate = currentDateTime;
                content.PublishDate = publishDate;
                content.CreatorId = creator.UserId;
                content.Version = 0;
                artifact.Contents.Add(content);
            }
            else
            {
                CreateContent(contentFile, artifact, publishDate, creator);
            }

            db.Artifacts.Add(artifact);
            db.Contents.Add(artifact.Contents.First());
            db.SaveChanges();
            return artifact;
        }

        private Content CreateContent(HttpPostedFileBase contentFile, Artifact artifact, DateTime publishDate, User creator)
        {
            CloudBlockBlob contentBlob;
            Content content = new Content();

            if (contentFile != null && contentFile.ContentLength != 0)
            {
                contentBlob = BlobUtils.UploadAndSaveBlob(cmsContentBlobContainer, contentFile);

                content.Artifact = artifact;
                content.ArtifactId = artifact.ArtifactId;
                content.ContentUrl = contentBlob.Uri.ToString();
                content.CreatorId = creator.UserId;
                content.CreateDate = DateTime.Now.Date;
                content.PublishDate = publishDate;
                if (artifact.Version > 0)
                {
                    artifact.Version++;
                }
                content.Version = artifact.Version;
            }

            return content;
        }
    }
}