using System;
using System.Web.Mvc;
using UberDeployer.Agent.Proxy;
using UberDeployer.WebApp.Core.Models.Dashboard;

namespace UberDeployer.WebApp.Core.Controllers
{
  public class DashboardController : UberDeployerWebAppController
  {
    private readonly IAgentService _agentService;

    public DashboardController(IAgentService agentService)
    {
      if (agentService == null)
      {
        throw new ArgumentNullException("agentService");
      }
      
      _agentService = agentService;
    }

    public DashboardController()
      : this(new AgentServiceClient())
    {
    }

    [HttpGet]
    public ActionResult Index()
    {
      var viewModel =
        new IndexViewModel();

      return View(viewModel);
    }
  }
}
