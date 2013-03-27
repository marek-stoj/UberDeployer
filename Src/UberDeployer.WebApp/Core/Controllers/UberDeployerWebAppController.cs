using System;
using System.Threading;
using System.Web.Mvc;

namespace UberDeployer.WebApp.Core.Controllers
{
  public abstract class UberDeployerWebAppController : Controller
  {
    protected HttpStatusCodeResult BadRequest()
    {      
      return new HttpStatusCodeResult(400, "400 - Bad Request");
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
