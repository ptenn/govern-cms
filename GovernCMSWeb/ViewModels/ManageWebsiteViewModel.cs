using System.Collections.Generic;
using GovernCMS.Models;

namespace GovernCMS.ViewModels
{
    public class ManageWebsiteViewModel
    {
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public int OwnerId { get; set; }

        public IList<Website> Websites { get; set; }
    }
}