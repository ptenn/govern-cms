using System.Web.Mvc;

namespace GovernCMS.ViewModels
{
    public abstract class WebsiteComponentViewModel
    {
        public int WebsiteId { get; set; }
        public SelectList WebsiteSelectList { get; set; }

    }
}