using System.Collections.Generic;

namespace GovernCMS.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public virtual ICollection<CategoryViewModel> SubCategories { get; set; }
    }
}