using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UberDeployer.Agent.Proxy;
using UberDeployer.Agent.Proxy.Dto;
using UberDeployer.WebApp.Core.Models.Api;
using UberDeployer.WebApp.Core.Services;
using UberDeployer.WebApp.Core.Utils;

namespace UberDeployer.WebApp.Core.Controllers
{
  public class ApiController : UberDeployerWebAppController
  {
    private const int _MaxProjectConfigurationBuildsCount = 10;

    private readonly ISessionService _sessionService;
    private readonly IAgentService _agentService;

    public ApiController(ISessionService sessionService, IAgentService agentService)
    {
      if (sessionService == null)
      {
        throw new ArgumentNullException("sessionService");
      }

      if (agentService == null)
      {
        throw new ArgumentNullException("agentService");
      }

      _sessionService = sessionService;
      _agentService = agentService;
    }

    public ApiController()
      : this(new SessionService(), new AgentServiceClient())
    {
    }

    [HttpGet]
    public ActionResult GetEnvironments()
    {
      List<EnvironmentViewModel> environmentViewModels =
        _agentService.GetEnvironmentInfos()
          .Select(pi => new EnvironmentViewModel { Name = pi.Name })
          .ToList();

      return
        Json(
          new { environments = environmentViewModels },
          JsonRequestBehavior.AllowGet);
    }


    [HttpGet]
    public ActionResult GetProjects()
    {
      List<ProjectViewModel> projectViewModels =
        _agentService.GetProjectInfos(ProjectFilter.Empty)
          .Select(pi => new ProjectViewModel { Name = pi.Name })
          .ToList();

      return
        Json(
          new { projects = projectViewModels },
          JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    public ActionResult GetProjectConfigurations(string projectName)
    {
      if (string.IsNullOrEmpty(projectName))
      {
        return BadRequest();
      }

      List<ProjectConfigurationViewModel> projectConfigurationViewModels =
        _agentService.GetProjectConfigurations(projectName, ProjectConfigurationFilter.Empty)
          .Select(pc => new ProjectConfigurationViewModel { Name = pc.Name })
          .ToList();

      return
        Json(
          new { projectConfigurations = projectConfigurationViewModels },
          JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    public ActionResult GetProjectConfigurationBuilds(string projectName, string projectConfigurationName)
    {
      if (string.IsNullOrEmpty(projectName))
      {
        return BadRequest();
      }

      if (string.IsNullOrEmpty(projectConfigurationName))
      {
        return BadRequest();
      }

      List<ProjectConfigurationBuildViewModel> projectConfigurationBuildViewModels =
        _agentService.GetProjectConfigurationBuilds(projectName, projectConfigurationName, _MaxProjectConfigurationBuildsCount, ProjectConfigurationBuildFilter.Empty)
          .Select(
            pcb =>
            new ProjectConfigurationBuildViewModel
              {
                Id = pcb.Id,
                Number = pcb.Number,
                Status = pcb.Status.ToString(),
                StartDateStr = pcb.StartDate
              })
          .ToList();

      return
        Json(
          new { projectConfigurationBuilds = projectConfigurationBuildViewModels },
          JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    public ActionResult GetDiagnosticMessages(long? lastSeenMaxMessageId)
    {
      if (!lastSeenMaxMessageId.HasValue)
      {
        return BadRequest();
      }

      List<DiagnosticMessageViewModel> diagnosticMessageViewModels =
        _agentService.GetDiagnosticMessages(
          _sessionService.UniqueClientId,
          lastSeenMaxMessageId.Value,
          DiagnosticMessageType.Trace)
          .Select(
            dm =>
            new DiagnosticMessageViewModel
              {
                MessageId = dm.MessageId,
                Message = dm.Message,
                Type = dm.Type.ToString(),
              }).ToList();

      return
        Json(
          new { messages = diagnosticMessageViewModels },
          JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public ActionResult Deploy(string projectName, string projectConfigurationName, string projectConfigurationBuildId, string targetEnvironmentName)
    {
      if (string.IsNullOrEmpty(projectName))
      {
        return BadRequest();
      }

      if (string.IsNullOrEmpty(projectConfigurationName))
      {
        return BadRequest();
      }

      if (string.IsNullOrEmpty(projectConfigurationBuildId))
      {
        return BadRequest();
      }

      if (string.IsNullOrEmpty(targetEnvironmentName))
      {
        return BadRequest();
      }

      try
      {
        _agentService.DeployAsync(
          _sessionService.UniqueClientId,
          SecurityUtils.CurrentUsername,
          projectName,
          projectConfigurationName,
          projectConfigurationBuildId,
          targetEnvironmentName);

        return Json(new { status = "OK" });
      }
      catch (Exception exc)
      {
        return Json(new { status = "FAIL", errorMessage = exc.Message });
      }
    }
  }
}
