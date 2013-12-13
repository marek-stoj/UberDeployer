using System;
using System.Reflection;
using System.Threading;
using System.Web.Mvc;
using log4net;
using UberDeployer.Common;

namespace UberDeployer.WebApp.Core.Controllers
{
  public abstract class UberDeployerWebAppController : Controller
  {
    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    protected HttpStatusCodeResult BadRequest()
    {      
      return new HttpStatusCodeResult(400, "400 - Bad Request");
    }

    protected HttpStatusCodeResult AccessDenied()
    {
      return new HttpStatusCodeResult(401, "401 - Unauthorized");
    }

    protected override void OnException(ExceptionContext filterContext)
    {
      _log.ErrorIfEnabled(() => "Unhandled exception.", filterContext.Exception);
    }

    protected static string UserIdentity
    {
      get
      {
        string userIdentity =
          Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity != null
            ? Thread.CurrentPrincipal.Identity.Name
            : null;

        if (string.IsNullOrEmpty(userIdentity))
        {
          throw new InvalidOperationException("No user identity!");
        }

        return userIdentity;
      }
    }
  }
}
