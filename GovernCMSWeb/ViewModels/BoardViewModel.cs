using System.Collections.Generic;
using System.Web.Mvc;
using GovernCMS.Models;

namespace GovernCMS.ViewModels
{
    public class BoardViewModel
    {
        public int WebsiteId { get; set; }
        public SelectList WebsiteSelectList { get; set; }

        public int BoardId { get; set; }
        public string BoardName { get; set; }
        public SelectList BoardSelectList { get; set; }

        public IList<BoardCard> BoardCards { get; set; }
    }
}