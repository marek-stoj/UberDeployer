using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UberDeployer.Agent.Proxy;
using UberDeployer.WebApp.Core.Models.History;

namespace UberDeployer.WebApp.Core.Controllers
{
  public class HistoryController : UberDeployerWebAppController
  {
    private const int _MaxDeploymentRequestsCount = 100;

    private readonly IAgentService _agentService;

    public HistoryController(IAgentService agentService)
    {
      if (agentService == null)
      {
        throw new ArgumentNullException("agentService");
      }
      
      _agentService = agentService;
    }

    public HistoryController()
      : this(new AgentServiceClient())
    {
    }

    [HttpGet]
    public ActionResult Index()
    {
      List<DeploymentRequestViewModel> deploymentRequestViewModels =
        _agentService.GetDeploymentRequests(0, _MaxDeploymentRequestsCount)
          .Select(
            dr =>
            new DeploymentRequestViewModel
              {
                DateRequested = dr.DateRequested,
                RequesterIdentity = dr.RequesterIdentity,
                ProjectName = dr.ProjectName,
                TargetEnvironmentName = dr.TargetEnvironmentName,
                FinishedSuccessfully = dr.FinishedSuccessfully,
              })
          .ToList();

      var viewModel =
        new IndexViewModel
          {
            DeploymentRequests = deploymentRequestViewModels,
          };

      return View(viewModel);
    }
  }
}
