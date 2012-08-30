using System.Threading;
using System.Web.Mvc;

namespace UberDeployer.WebApp.Core.Controllers
{
  public abstract class UberDeployerWebAppController : Controller
  {
    protected ActionResult BadRequest()
    {
      Response.StatusCode = 400;

      return Content("400 - Bad Request");
    }

    protected string CurrentUsername
    {
      get
      {
        return Thread.CurrentPrincipal.Identity.Name;
      }
    }
  }
}
