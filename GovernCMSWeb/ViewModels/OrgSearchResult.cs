using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GovernCMS.ViewModels
{
    public class OrgSearchResult
    {
        public bool Match { get; set; }
        public int OrgId { get; set; }
        public string OrgName { get; set; }
        public IList<String> AllOrgNames { get; set; }
    }
}