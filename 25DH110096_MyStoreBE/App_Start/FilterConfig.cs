using System.Web;
using System.Web.Mvc;

namespace _25DH110096_MyStoreBE
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
