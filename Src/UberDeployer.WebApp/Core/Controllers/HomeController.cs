using System;
using System.Web.Mvc;
using UberDeployer.Agent.Proxy;

namespace UberDeployer.WebApp.Core.Controllers
{
  public class HomeController : UberDeployerWebAppController
  {
    private readonly IAgentService _agentService;

    public HomeController(IAgentService agentService)
    {
      if (agentService == null)
      {
        throw new ArgumentNullException("agentService");
      }
      
      _agentService = agentService;
    }

    public HomeController()
      : this(new AgentServiceClient())
    {
    }

    [HttpGet]
    public ActionResult Index()
    {
      return RedirectToAction("Index", "Deployment");
    }
  }
}
