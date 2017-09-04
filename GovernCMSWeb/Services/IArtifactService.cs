using System;
using System.Collections.Generic;
using System.Web;
using GovernCMS.Models;

namespace GovernCMS.Services
{
    /// <summary>
    /// Artifact Service Contract
    /// </summary>
    interface IArtifactService
    {
        /// <summary>
        /// Create an Artifact Entity from a Content File (such as a Word Doc or PDF)
        /// </summary>
        /// <param name="artifactName">The Artifact Name</param>
        /// <param name="description">The Artifact Description</param>
        /// <param name="contentFile">The Content URL for the first Content Version of the Artifact</param>
        /// <param name="publishDate">Publish Date for the Content, defaults to today</param>
        /// <param name="creator">The User who is creating the Artifact</param>
        /// <returns>Newly-created Artifact.  By default, it will be accessible to the User and the User's Group.</returns>
        Artifact CreateArtifactFromFile(string artifactName, string description, HttpPostedFileBase contentFile, 
            DateTime publishDate, User creator);

        /// <summary>
        /// Create an Artifact Entity from Content (such as a Webpage)
        /// </summary>
        /// <param name="artifactName">The Artifact Name</param>
        /// <param name="description">The Artifact Description</param>
        /// <param name="content">The Content for the first Content Version of the Artifact</param>
        /// <param name="publishDate">Publish Date for the Content, defaults to today</param>
        /// <param name="creator">The User who is creating the Artifact</param>
        /// <returns>Newly-created Artifact.  By default, it will be accessible to the User and the User's Group.</returns>
        Artifact CreateArtifactFromContent(string artifactName, string description, string content, 
            DateTime publishDate, User creator);

        /// <summary>
        /// Find all Artifacts for a User
        /// </summary>
        /// <param name="user">The User for retrieving all Artifacts</param>
        /// <returns>List of User's Artifacts</returns>
        IList<Artifact> FindArtifactsByUser(User user);

        /// <summary>
        /// Find all Artifacts for a Group
        /// </summary>
        /// <param name="group">The Group for retrieving all Artifacts</param>
        /// <returns>List of Group's Artifacts</returns>
        IList<Artifact> FindArtifactsByGroup(Group group);

        /// <summary>
        /// Find an Artifact by Unique ID
        /// </summary>
        /// <param name="id">Unique ID of the Artifact</param>
        /// <param name="includeContent">True if Content should be eager fetched, false otherwise</param>
        /// <returns>The Artifact by Unique ID, null if not found</returns>
        Artifact FindArtifactById(int id, bool includeContent);
        
        /// <summary>
        /// Update an Artifact to persistent store
        /// </summary>
        /// <param name="artifact">The Artifact to Update</param>
        /// <returns></returns>
        Artifact UpdateArtifact(Artifact artifact);

        /// <summary>
        /// Adds Content to an existing Artifact
        /// </summary>
        /// <param name="artifact">The Artifact where Content will be added</param>
        /// <param name="contentFile">The Content File either this or HTML should be provided, but not both</param>
        /// <param name="contentHtml">The Content HTML, either this or File should be provided, but not both</param>
        /// <param name="publishDate">Publish Date for the Content, defaults to today</param>
        /// <param name="creator">User who is creating the Content</param>
        /// <returns>Artifact with new Content added</returns>
        Artifact AddContentToArtifact(Artifact artifact, HttpPostedFileBase contentFile, string contentHtml,
            DateTime publishDate, User creator);
    }
}
