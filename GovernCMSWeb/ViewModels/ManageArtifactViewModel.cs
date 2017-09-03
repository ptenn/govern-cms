using System.Collections.Generic;
using GovernCMS.Models;

namespace GovernCMS.ViewModels
{
    public class ManageArtifactViewModel
    {
        /// <summary>
        /// Artifact ID.  If it is null, we are creating a new Artifact, otherwise we are editing an existing Artifact.
        /// </summary>
        public int? ArtifactId { get; set; }

        public int Version { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int OwnerId { get; set; }

        public int OrganizationId { get; set; }

        public IList<Content> ContentItems { get; set; }

        public string ContentHtml { get; set; }

        public string ContentUrl { get; set; }

        public string ContentOrigFileName { get; set; }

        public bool HasContent()
        {
            return ContentItems != null && ContentItems.Count > 0;
        }
    }
}