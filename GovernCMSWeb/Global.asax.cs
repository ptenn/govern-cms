using System.Web.Mvc;
using System.Web.Routing;
using GovernCMSWeb;
using log4net.Config;

namespace GovernCMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            XmlConfigurator.Configure();

        }
    }
}
