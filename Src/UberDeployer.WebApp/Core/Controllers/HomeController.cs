using System.Web.Mvc;
using UberDeployer.Agent.Proxy;

namespace UberDeployer.WebApp.Core.Controllers
{
  public class HomeController : UberDeployerWebAppController
  {
    [HttpGet]
    public ActionResult Index()
    {
      var agentServiceClient = new AgentServiceClient();

      return Content(agentServiceClient.SayHello("Mark"));
    }
  }
}
