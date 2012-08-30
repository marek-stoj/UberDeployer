using System;
using System.Web.Mvc;
using UberDeployer.Agent.Proxy;
using UberDeployer.WebApp.Core.Models.Deployment;
using UberDeployer.WebApp.Core.Services;

namespace UberDeployer.WebApp.Core.Controllers
{
  public class DeploymentController : UberDeployerWebAppController
  {
    private readonly IAgentService _agentService;

    public DeploymentController(IAgentService agentService)
    {
      if (agentService == null)
      {
        throw new ArgumentNullException("agentService");
      }
      
      _agentService = agentService;
    }

    public DeploymentController()
      : this(new AgentServiceClient())
    {
    }

    [HttpGet]
    public ActionResult Index()
    {
      var viewModel =
        new IndexViewModel
          {
            TipOfTheDay = LifeProFuckingTips.GetTodayTip(),
          };

      return View(viewModel);
    }
  }
}
