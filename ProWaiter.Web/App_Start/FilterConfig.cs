using ProWaiter.Web.Util;
using System.Web;
using System.Web.Mvc;

namespace ProWaiter.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ValidadorActionFilterAttribute());
        }
    }
}
