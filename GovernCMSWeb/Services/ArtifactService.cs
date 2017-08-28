using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using GovernCMS.Models;

namespace GovernCMS.Services
{
    public class ArtifactService : IArtifactService
    {
        private GovernCmsContext db = new GovernCmsContext();

        public Artifact CreateArtifactFromUrl(string artifactName, string description, string url, User creator)
        {
            Artifact artifact = CreateArtifact(artifactName, description, url, null, creator);

            db.Artifacts.Add(artifact);            
            db.Contents.Add(artifact.Contents.First());
            db.SaveChanges();

            return artifact;
        }

        public Artifact CreateArtifactFromContent(string artifactName, string description, string content, User creator)
        {
            Artifact artifact = CreateArtifact(artifactName, description, null, content, creator);

            db.Artifacts.Add(artifact);
            db.Contents.Add(artifact.Contents.First());
            db.SaveChanges();

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

        public Artifact FindArtifactById(int id)
        {
            return db.Artifacts.Find(id);
        }

        public Artifact UpdateArtifact(Artifact artifact)
        {
            db.Artifacts.Attach(artifact);
            db.Entry(artifact).State = EntityState.Modified;
            db.SaveChanges();
            return artifact;
        }

        public Artifact AddContentToArtifact(Artifact artifact, string contentUrl, string contentHtml)
        {
            DateTime currentDateTime = DateTime.Now;

            Content content = new Content();
            content.Artifact = artifact;
            content.ContentUrl = contentUrl;
            content.ContentHtml = contentHtml;
            content.CreateDate = currentDateTime;
            content.UpdateDate = currentDateTime;
            content.Version = 1;

            db.Contents.Add(content);
            db.SaveChanges();
            artifact.Contents.Add(content);

            return artifact;
        }

        private Artifact CreateArtifact(string artifactName, string description, string url, string contentHtml,
            User creator)
        {
            DateTime currentDateTime = DateTime.Now;
            Artifact artifact = new Artifact();
            artifact.Name = artifactName;
            artifact.Description = description;
            artifact.CreateDate = currentDateTime;

            artifact.OwnerId = creator.UserId;

            Content content = new Content();
            content.Artifact = artifact;
            content.ContentUrl = url;
            content.ContentHtml = contentHtml;
            content.CreateDate = currentDateTime;
            content.UpdateDate = currentDateTime;
            content.Version = 0;

            if (artifact.Contents == null)
            {
                artifact.Contents = new List<Content>();
            }
            return artifact;
        }
    }
}