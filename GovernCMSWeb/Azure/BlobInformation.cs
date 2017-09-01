using System;
using System.IO;

namespace GovernCMS.Azure
{
    public class BlobInformation
    {
        public Uri BlobUri { get; set; }

        public string BlobName
        {
            get
            {
                return BlobUri.Segments[BlobUri.Segments.Length - 1];
            }
        }
        public string BlobNameWithoutExtension
        {
            get
            {
                return Path.GetFileNameWithoutExtension(BlobName);
            }
        }

        public string BlobNameExtension
        {
            get
            {
                return Path.GetExtension(BlobName);
            }
        }

        public int Id { get; set; }        

        public IdType Type { get; set; }
    }

}
