using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GovernCMS.Models;

namespace GovernCMS.ViewModels
{
    public class BreadcrumbViewModel
    {
        public int WebsiteId { get; set; }
        public SelectList WebsiteSelectList { get; set; }
        public IList<Category> Categories { get; set; }
    }
}