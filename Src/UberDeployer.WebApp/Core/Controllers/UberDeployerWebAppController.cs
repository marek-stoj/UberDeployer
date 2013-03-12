using System.Web.Mvc;

namespace UberDeployer.WebApp.Core.Controllers
{
  public abstract class UberDeployerWebAppController : Controller
  {
    protected HttpStatusCodeResult BadRequest()
    {      
      return new HttpStatusCodeResult(400, "400 - Bad Request");
    }    
  }
}
