using System.Web.Mvc;

namespace UberDeployer.WebApp.Core.Controllers
{
  public class HomeController : UberDeployerWebAppController
  {
    [HttpGet]
    public ActionResult Index()
    {
      return RedirectToAction("Index", "Dashboard");
    }
  }
}
