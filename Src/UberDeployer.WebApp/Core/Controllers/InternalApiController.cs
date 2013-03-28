using System;
using System.Web.Mvc;
using UberDeployer.Agent.Proxy;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.WebApp.Core.Connectivity;
using UberDeployer.WebApp.Core.Services;

namespace UberDeployer.WebApp.Core.Controllers
{
  public class InternalApiController : UberDeployerWebAppController
  {
    private readonly IAgentService _agentService;
    private readonly IDeploymentStateProvider _deploymentStateProvider;

    public InternalApiController(IAgentService agentService, IDeploymentStateProvider deploymentStateProvider)
    {
      Guard.NotNull(agentService, "agentService");
      Guard.NotNull(deploymentStateProvider, "deploymentStateProvider");

      _agentService = agentService;
      _deploymentStateProvider = deploymentStateProvider;
    }

    public InternalApiController()
      : this(new AgentServiceClient(), new DeploymentStateProvider())
    {
    }

    [HttpGet]
    public ActionResult CollectCredentials(Guid? deploymentId, string environmentName, string machineName, string username)
    {
      if (!deploymentId.HasValue)
      {
        return BadRequest();
      }

      if (string.IsNullOrEmpty(username))
      {
        return BadRequest();
      }

      DeploymentState deploymentState =
        _deploymentStateProvider.FindDeploymentState(deploymentId.Value);

      if (deploymentState == null)
      {
        return Content("FAIL");
      }

      DeploymentHub.PromptForCredentials(
        deploymentId.Value,
        deploymentState.UserIdentity,
        deploymentState.ProjectName,
        deploymentState.ProjectConfigurationName,
        environmentName,
        machineName,
        username);

      return Content("OK");
    }

    [HttpPost]
    public ActionResult OnCredentialsCollected(Guid? deploymentId, string password)
    {
      if (!deploymentId.HasValue)
      {
        return BadRequest();
      }

      if (string.IsNullOrEmpty(password))
      {
        return BadRequest();
      }

      _agentService.SetCollectedCredentialsForAsynchronousWebCredentialsCollector(
        deploymentId.Value,
        password);

      return Content("OK");
    }

    [HttpGet]
    public ActionResult OnCollectCredentialsTimedOut(Guid? deploymentId)
    {
      if (!deploymentId.HasValue)
      {
        return BadRequest();
      }

      DeploymentState deploymentState =
        _deploymentStateProvider.FindDeploymentState(deploymentId.Value);

      if (deploymentState == null)
      {
        return Content("FAIL");
      }

      DeploymentHub.CancelPromptForCredentials(deploymentState.UserIdentity);

      return Content("OK");
    }
  }
}
