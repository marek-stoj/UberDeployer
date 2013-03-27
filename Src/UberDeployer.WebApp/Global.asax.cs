using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UberDeployer.Common;
using UberDeployer.WebApp.Core.Connectivity;
using log4net;
using log4net.Config;

namespace UberDeployer.WebApp
{
  public class MvcApplication : HttpApplication
  {
    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute(
        "Default",
        "{controller}/{action}/{id}",
        new { controller = "Home", action = "Index", id = UrlParameter.Optional });
    }

    protected void Application_Start()
    {
      GlobalContext.Properties["applicationName"] = "UberDeployer.WebApp";
      XmlConfigurator.Configure();

      RouteTable.Routes.MapConnection<MyPersistentConnection>("SignalR", "SignalR");

      AreaRegistration.RegisterAllAreas();

      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);
      
      _log.InfoIfEnabled(() => "Application has started.");
    }

    protected void Application_Error()
    {
      Exception exception = Server.GetLastError();
      HttpException httpException = exception as HttpException;

      if (httpException != null)
      {
        if (httpException.GetHttpCode() == 404)
        {
          return;
        }
      }

      _log.ErrorIfEnabled(() => "Unhandled exception.", exception);
    }
  }
}
