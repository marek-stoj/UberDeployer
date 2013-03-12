using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;
using UberDeployer.Agent.Proxy;
using UberDeployer.Agent.Proxy.Dto;
using UberDeployer.Agent.Proxy.Faults;
using UberDeployer.WebApp.Core.Models.Api;
using UberDeployer.WebApp.Core.Services;
using UberDeployer.WebApp.Core.Utils;

namespace UberDeployer.WebApp.Core.Controllers
{
  public class ApiController : UberDeployerWebAppController
  {
    private const int _MaxProjectConfigurationBuildsCount = 10;
    private const string _AppSettingsKey_AllowedEnvironments = "AllowedEnvironments";
    private const string _AppSettingsKey_AllowedProjectConfigurations = "AllowedProjectConfigurations";

    private static readonly ISet<string> _allowedEnvironments;
    private static readonly ISet<string> _allowedProjectConfigurations;

    private readonly ISessionService _sessionService;
    private readonly IAgentService _agentService;

    static ApiController()
    {
      string allowedEnvironmentsStr = ConfigurationManager.AppSettings[_AppSettingsKey_AllowedEnvironments];

      if (string.IsNullOrEmpty(allowedEnvironmentsStr))
      {
        throw new ConfigurationErrorsException(string.Format("Missing app setting. Key: {0}.", _AppSettingsKey_AllowedEnvironments));
      }

      string allowedProjectConfigurationsStr = ConfigurationManager.AppSettings[_AppSettingsKey_AllowedProjectConfigurations];

      if (string.IsNullOrEmpty(allowedProjectConfigurationsStr))
      {
        throw new ConfigurationErrorsException(string.Format("Missing app setting. Key: {0}.", _AppSettingsKey_AllowedProjectConfigurations));
      }

      _allowedEnvironments = ParseAppSettingSet(allowedEnvironmentsStr);
      _allowedProjectConfigurations = ParseAppSettingSet(allowedProjectConfigurationsStr);
    }

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
          .Where(pi => _allowedEnvironments.Count == 0 || _allowedEnvironments.Contains(pi.Name))
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
          .Select(pi => new ProjectViewModel { Name = pi.Name, Type = pi.Type })
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
          .Where(pc => _allowedProjectConfigurations.Count == 0 || _allowedProjectConfigurations.Contains(pc.Name))
          .Select(pc => new ProjectConfigurationViewModel { Name = pc.Name })
          .ToList();

      return
        Json(
          new { projectConfigurations = projectConfigurationViewModels },
          JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    public ActionResult
      GetProjectConfigurationBuilds(string projectName, string projectConfigurationName)
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
    public ActionResult GetWebMachineNames(string envName)
    {
      if (string.IsNullOrWhiteSpace(envName))
      {
        return BadRequest();
      }

      try
      {
        return Json(
          _agentService.GetWebMachinesNames(envName),
          JsonRequestBehavior.AllowGet);
      }
      catch (FaultException<EnvironmentNotFoundFault>)
      {
        return BadRequest();
      }      
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
    public ActionResult Deploy(string projectName, string projectConfigurationName, string projectConfigurationBuildId, string targetEnvironmentName, List<string> targetMachines)
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
          new UberDeployer.Agent.Proxy.Dto.DeploymentInfo
            {
              ProjectName = projectName,
              ProjectConfigurationName = projectConfigurationName,
              ProjectConfigurationBuildId = projectConfigurationBuildId,
              TargetEnvironmentName = targetEnvironmentName
            });

        return Json(new { status = "OK" });
      }
      catch (Exception exc)
      {
        return Json(new { status = "FAIL", errorMessage = exc.Message });
      }
    }

    private static ISet<string> ParseAppSettingSet(string appSettingValue)
    {
      if (string.IsNullOrEmpty(appSettingValue))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "appSettingValue");
      }

      if (appSettingValue == "*")
      {
        return new HashSet<string>();
      }

      return new HashSet<string>(appSettingValue.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
    }
  }
}
