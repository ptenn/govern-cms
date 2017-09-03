using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using log4net;
using log4net.Config;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

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

            logger.Info("Creating cms blob container");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var cmsBlobContainer = blobClient.GetContainerReference("cmsartifacts");
            if (cmsBlobContainer.CreateIfNotExists())
            {
                // Enable public access on the newly created "images" container.
                cmsBlobContainer.SetPermissions(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    });
            }
            logger.Info("Storage initialized");
        }
    }
}
