using System;
using System.Collections.Generic;

namespace GovernCMS.ViewModels
{
    public class OrgSearchResultViewModel
    {
        public bool Match { get; set; }
        public int OrgId { get; set; }
        public string OrgName { get; set; }
        public IList<String> AllOrgNames { get; set; }
    }
}