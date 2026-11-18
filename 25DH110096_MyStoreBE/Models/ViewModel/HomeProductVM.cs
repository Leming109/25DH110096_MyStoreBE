using _25DH110096_MyStoreBE.Models;
using PagedList;
using System.Collections.Generic;

namespace _25DH110096_MyStoreBE.Models.ViewModel
{
    public class HomeProductVM
    {
        public string SearchTerm { get; set; }

        public string SearchString
        {
            get { return SearchTerm; }
            set { SearchTerm = value; }
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 6;
        public List<Product> FeaturedProducts { get; set; }
        public IPagedList<Product> NewProducts { get; set; }
    }
}

