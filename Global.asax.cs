using System.Web.Mvc;
using System.Web.Routing;

namespace AuthiQ.SSO
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Developer-JavaScript", // Route name
                "Developer/JavaScript/Demonstration", // URL with parameters
                new { controller = "Developer", action = "Demonstration", type = "js" } // Parameter defaults
                , new string[] { "AuthiQ.SSO.Controllers" }
            );
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Site", action = "Index", id = UrlParameter.Optional } // Parameter defaults
                , new string[] { "AuthiQ.SSO.Controllers" }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}