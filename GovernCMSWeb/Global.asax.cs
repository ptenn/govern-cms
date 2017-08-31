using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using log4net;
using log4net.Config;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace GovernCMS
{
    public class MvcApplication : System.Web.HttpApplication
    {

        private static ILog logger = LogManager.GetLogger(typeof(MvcApplication));

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            XmlConfigurator.Configure();
            InitializeStorage();
        }

        private void InitializeStorage()
        {
            // Open storage account using credentials from .cscfg file.
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["GovernCmsStorage"].ConnectionString);

            logger.Debug("Creating cms blob container");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var cmsArtifactsBlobContainer = blobClient.GetContainerReference("cmsArtifacts");
            if (cmsArtifactsBlobContainer.CreateIfNotExists())
            {
                // Enable public access on the newly created "agendas" container.
                cmsArtifactsBlobContainer.SetPermissions(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });
            }
            logger.Debug("Storage initialized");
        }
    }
}
