using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using log4net;
using Microsoft.WindowsAzure.Storage.Blob;

namespace GovernCMS.Azure
{
    public static class BlobUtils
    {
        private static ILog logger = LogManager.GetLogger(typeof(BlobUtils));
        
        public static async Task<CloudBlockBlob> UploadAndSaveBlobAsync(CloudBlobContainer blobContainer, HttpPostedFileBase documentFile)
        {
            logger.Info(String.Format("Uploading image file {0}", documentFile.FileName));

            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(documentFile.FileName);
            // Retrieve reference to a blob. 
            CloudBlockBlob imageBlob = blobContainer.GetBlockBlobReference(blobName);
            // Create the blob by uploading a local file.
            using (var fileStream = documentFile.InputStream)
            {
                await imageBlob.UploadFromStreamAsync(fileStream);
            }

            logger.Info(String.Format("Uploaded image file to {0}", imageBlob.Uri));

            return imageBlob;
        }

        public static CloudBlockBlob UploadAndSaveBlob(CloudBlobContainer blobContainer, Stream stream, String extension)
        {
            string blobName = Guid.NewGuid().ToString() + extension;
            // Retrieve reference to a blob. 
            CloudBlockBlob imageBlob = blobContainer.GetBlockBlobReference(blobName);
            // Create the blob by uploading a local file.
            imageBlob.UploadFromStream(stream);

            logger.Info(String.Format("Uploaded image file to {0}", imageBlob.Uri));

            return imageBlob;
        }

        public static CloudBlockBlob UploadAndSaveBlob(CloudBlobContainer blobContainer, String text, String contentType, String extension)
        {
            string blobName = Guid.NewGuid().ToString() + extension;
            // Retrieve reference to a blob. 
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobName);
            // Create the blob by uploading a local file.
            blob.UploadText(text);
            // Set the Blob content type
            blob.Properties.ContentType = contentType;
            blob.SetProperties();

            logger.Info(String.Format("Uploaded text file to {0}", blob.Uri));

            return blob;
        }


        public static CloudBlockBlob UploadAndSaveBlob(CloudBlobContainer blobContainer, HttpPostedFileBase documentFile)
        {
            logger.Info(String.Format("Uploading image file {0}", documentFile.FileName));

            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(documentFile.FileName);
            // Retrieve reference to a blob. 
            CloudBlockBlob imageBlob = blobContainer.GetBlockBlobReference(blobName);
            // Create the blob by uploading a local file.
            using (var fileStream = documentFile.InputStream)
            {
                imageBlob.UploadFromStream(fileStream);
            }

            logger.Info(String.Format("Uploaded image file to {0}", imageBlob.Uri));

            return imageBlob;
        }

        public static CloudBlockBlob UploadAndSaveBlob(CloudBlobContainer blobContainer, HttpPostedFileBase documentFile, String mimeType)
        {
            logger.Info(String.Format("Uploading image file {0}", documentFile.FileName));

            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(documentFile.FileName);
            // Retrieve reference to a blob. 
            CloudBlockBlob imageBlob = blobContainer.GetBlockBlobReference(blobName);
            imageBlob.Properties.ContentType = mimeType;
            // Create the blob by uploading a local file.
            using (var fileStream = documentFile.InputStream)
            {
                imageBlob.UploadFromStream(fileStream);
            }

            logger.Info(String.Format("Uploaded image file to {0}", imageBlob.Uri));

            return imageBlob;
        }

        public static CloudBlockBlob UploadAndSaveBlob(CloudBlobContainer blobContainer, byte[] byteArray, String fileName)
        {
            logger.Info(String.Format("Uploading image file {0}", fileName));

            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            // Retrieve reference to a blob. 
            CloudBlockBlob imageBlob = blobContainer.GetBlockBlobReference(blobName);
            // Create the blob by uploading a local file.
            using (var fileStream = new MemoryStream(byteArray))
            {
                imageBlob.UploadFromStream(fileStream);
            }

            logger.Info(String.Format("Uploaded image file to {0}", imageBlob.Uri));

            return imageBlob;
        }
    }
}