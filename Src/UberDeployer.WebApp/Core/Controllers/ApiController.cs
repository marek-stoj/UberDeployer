using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using UberDeployer.Agent.Proxy;
using UberDeployer.Agent.Proxy.Dto;
using UberDeployer.Agent.Proxy.Dto.Input;
using UberDeployer.Agent.Proxy.Dto.Metadata;
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
          .Where(pi => _allowedEnvironments.Count == 0 || _allowedEnvironments.Any(ae => Regex.IsMatch(pi.Name, ae, RegexOptions.IgnoreCase)))
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
          .Select(CreateProjectViewModel)
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
          .Where(pc => _allowedProjectConfigurations.Count == 0 || _allowedProjectConfigurations.Any(apc => Regex.IsMatch(pc.Name, apc, RegexOptions.IgnoreCase)))
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
    public ActionResult GetWebMachineNames(string envName)
    {
      if (string.IsNullOrWhiteSpace(envName))
      {
        return BadRequest();
      }

      try
      {
        List<string> webMachineNames =
          _agentService.GetWebMachineNames(envName);

        return Json(
          webMachineNames,
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
    public ActionResult Deploy(string projectName, string projectConfigurationName, string projectConfigurationBuildId, string targetEnvironmentName, ProjectType? projectType, List<string> targetMachines = null)
    {
      return
        DoDeployOrSimulate(
          false,
          projectName,
          projectConfigurationName,
          projectConfigurationBuildId,
          targetEnvironmentName,
          projectType,
          targetMachines);
    }

    [HttpPost]
    public ActionResult Simulate(string projectName, string projectConfigurationName, string projectConfigurationBuildId, string targetEnvironmentName, ProjectType? projectType, List<string> targetMachines = null)
    {
      return
        DoDeployOrSimulate(
          true,
          projectName,
          projectConfigurationName,
          projectConfigurationBuildId,
          targetEnvironmentName,
          projectType,
          targetMachines);
    }

    [HttpGet]
    public ActionResult GetProjectMetadata(string projectName, string environmentName)
    {
      if (string.IsNullOrEmpty(projectName))
      {
        return BadRequest();
      }

      if (string.IsNullOrEmpty(environmentName))
      {
        return BadRequest();
      }

      try
      {
        ProjectMetadata projectMetadata =
          _agentService.GetProjectMetadata(projectName, environmentName);

        var projectMetadataViewModel =
          new ProjectMetadataViewModel
          {
            ProjectName = projectMetadata.ProjectName,
            EnvironmentName = projectMetadata.EnvironmentName,
            ProjectVersions =
              projectMetadata.ProjectVersions
                .Select(
                  pv =>
                  new MachineSpecificProjectVersionViewModel
                  {
                    MachineName = pv.MachineName,
                    ProjectVersion = pv.ProjectVersion,
                  }).ToList(),
          };

        projectMetadataViewModel.Status = "OK";

        return
          Json(
            projectMetadataViewModel,
            JsonRequestBehavior.AllowGet);
      }
      catch (Exception exc)
      {
        return HandleAjaxError(exc);
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

    private static ProjectViewModel CreateProjectViewModel(ProjectInfo pi)
    {
      return new ProjectViewModel
      {
        Name = pi.Name,
        Type = (ProjectTypeViewModel)Enum.Parse(typeof(ProjectTypeViewModel), pi.Type.ToString(), true),
      };
    }

    private ActionResult DoDeployOrSimulate(bool isSimulation, string projectName, string projectConfigurationName, string projectConfigurationBuildId, string targetEnvironmentName, ProjectType? projectType, List<string> targetMachines = null)
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

      if (!projectType.HasValue)
      {
        return BadRequest();
      }

      try
      {
        _agentService.DeployAsync(
          _sessionService.UniqueClientId,
          SecurityUtils.CurrentUsername,
          CreateDeploymentInfo(
            isSimulation,
            projectName,
            projectConfigurationName,
            projectConfigurationBuildId,
            targetEnvironmentName,
            projectType.Value,
            targetMachines));

        return Json(new { Status = "OK" });
      }
      catch (Exception exc)
      {
        return HandleAjaxError(exc);
      }
    }

    private ActionResult HandleAjaxError(Exception exception)
    {
      return Json(new { Status = "FAIL", ErrorMessage = exception.Message }, JsonRequestBehavior.AllowGet);
    }

    private DeploymentInfo CreateDeploymentInfo(bool isSimulation, string projectName, string projectConfigurationName, string projectConfigurationBuildId, string targetEnvironmentName, ProjectType projectType, IEnumerable<string> targetMachines = null)
    {
      return
        new DeploymentInfo
        {
          IsSimulation = isSimulation,
          ProjectName = projectName,
          ProjectConfigurationName = projectConfigurationName,
          ProjectConfigurationBuildId = projectConfigurationBuildId,
          TargetEnvironmentName = targetEnvironmentName,
          InputParams = CreateDeploymentInputParams(projectName, targetMachines),
        };
    }

    private InputParams CreateDeploymentInputParams(string projectName, IEnumerable<string> targetMachines = null)
    {
      ProjectType projectType = GetProjectType(projectName);

      switch (projectType)
      {
        case ProjectType.Db:
        {
          return new DbInputParams();
        }

        case ProjectType.NtService:
        {
          return new NtServiceInputParams();
        }

        case ProjectType.SchedulerApp:
        {
          return new SchedulerAppInputParams();
        }

        case ProjectType.TerminalApp:
        {
          return new TerminalAppInputParams();
        }

        case ProjectType.WebApp:
        {
          List<string> onlyIncludedWebMachines = null;

          if (targetMachines != null)
          {
            onlyIncludedWebMachines = new List<string>(targetMachines);

            if (onlyIncludedWebMachines.Count == 0)
            {
              throw new ArgumentException("If target machines are specified there must be at least one present.", "targetMachines");
            }
          }

          return
            new WebAppInputParams
            {
              OnlyIncludedWebMachines = onlyIncludedWebMachines,
            };
        }

        case ProjectType.WebService:
        {
          return new WebServiceInputParams();
        }

        default:
        {
          throw new NotSupportedException(string.Format("Unknown project type: '{0}'.", projectType));
        }
      }
    }

    private ProjectType GetProjectType(string projectName)
    {
      ProjectInfo projectInfo =
        _agentService.GetProjectInfos(
          new ProjectFilter { Name = projectName })
          .SingleOrDefault(pi => pi.Name == projectName);

      if (projectInfo == null)
      {
        throw new InternalException(string.Format("Unknown project: '{0}'.", projectName));
      }

      return projectInfo.Type;
    }
  }
}
