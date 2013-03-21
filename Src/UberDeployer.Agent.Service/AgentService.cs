using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using UberDeployer.Agent.Proxy;
using UberDeployer.Agent.Proxy.Dto.TeamCity;
using UberDeployer.Agent.Proxy.Faults;
using UberDeployer.Agent.Service.Diagnostics;
using UberDeployer.Common;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.CommonConfiguration;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Deployment.Pipeline;
using UberDeployer.Core.Deployment.Pipeline.Modules;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Metadata;
using UberDeployer.Core.TeamCity;
using log4net;
using DeploymentInfo = UberDeployer.Agent.Proxy.Dto.DeploymentInfo;
using DeploymentRequest = UberDeployer.Core.Deployment.Pipeline.Modules.DeploymentRequest;
using DiagnosticMessage = UberDeployer.Core.Deployment.DiagnosticMessage;
using EnvironmentInfo = UberDeployer.Core.Domain.EnvironmentInfo;
using MachineSpecificProjectVersion = UberDeployer.Agent.Proxy.Dto.Metadata.MachineSpecificProjectVersion;
using ProjectInfo = UberDeployer.Core.Domain.ProjectInfo;
using WebAppProjectInfo = UberDeployer.Core.Domain.WebAppProjectInfo;

namespace UberDeployer.Agent.Service
{
  public class AgentService : IAgentService
  {
    private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private readonly IDeploymentPipeline _deploymentPipeline;
    private readonly IProjectInfoRepository _projectInfoRepository;
    private readonly IEnvironmentInfoRepository _environmentInfoRepository;
    private readonly ITeamCityClient _teamCityClient; // TODO IMM HI: abstract away?
    private readonly IDeploymentRequestRepository _deploymentRequestRepository;
    private readonly IDiagnosticMessagesLogger _diagnosticMessagesLogger;
    private readonly IProjectMetadataExplorer _projectMetadataExplorer;

    #region Constructor(s)

    public AgentService(
      IDeploymentPipeline deploymentPipeline,
      IProjectInfoRepository projectInfoRepository,
      IEnvironmentInfoRepository environmentInfoRepository,
      ITeamCityClient teamCityClient,
      IDeploymentRequestRepository deploymentRequestRepository,
      IDiagnosticMessagesLogger diagnosticMessagesLogger,
      IProjectMetadataExplorer projectMetadataExplorer)
    {
      Guard.NotNull(deploymentPipeline, "deploymentPipeline");
      Guard.NotNull(projectInfoRepository, "projectInfoRepository");
      Guard.NotNull(environmentInfoRepository, "environmentInfoRepository");
      Guard.NotNull(teamCityClient, "teamCityClient");
      Guard.NotNull(deploymentRequestRepository, "deploymentRequestRepository");
      Guard.NotNull(diagnosticMessagesLogger, "diagnosticMessagesLogger");

      _projectInfoRepository = projectInfoRepository;
      _environmentInfoRepository = environmentInfoRepository;
      _teamCityClient = teamCityClient;
      _deploymentPipeline = deploymentPipeline;
      _deploymentRequestRepository = deploymentRequestRepository;
      _diagnosticMessagesLogger = diagnosticMessagesLogger;
      _projectMetadataExplorer = projectMetadataExplorer;
    }

    public AgentService()
      : this(
        ObjectFactory.Instance.CreateDeploymentPipeline(),
        ObjectFactory.Instance.CreateProjectInfoRepository(),
        ObjectFactory.Instance.CreateEnvironmentInfoRepository(),
        ObjectFactory.Instance.CreateTeamCityClient(),
        ObjectFactory.Instance.CreateDeploymentRequestRepository(),
        InMemoryDiagnosticMessagesLogger.Instance,
        ObjectFactory.Instance.CreateProjectMetadataExplorer())
    {
    }

    #endregion

    #region IAgentService Members

    public void Deploy(Guid uniqueClientId, string requesterIdentity, DeploymentInfo deploymentInfo)
    {
      Guard.NotNull(deploymentInfo, "DeploymentInfo");

      try
      {
        if (uniqueClientId == Guid.Empty)
        {
          throw new ArgumentException("Argument can't be Guid.Empty.", "uniqueClientId");
        }

        if (string.IsNullOrEmpty(requesterIdentity))
        {
          throw new ArgumentException("Argument can't be null nor empty.", "requesterIdentity");
        }

        ProjectInfo projectInfo =
          _projectInfoRepository.FindByName(deploymentInfo.ProjectName);

        if (projectInfo == null)
        {
          throw new FaultException<ProjectNotFoundFault>(new ProjectNotFoundFault { ProjectName = deploymentInfo.ProjectName });
        }

        DoDeploy(uniqueClientId, requesterIdentity, deploymentInfo, projectInfo);
      }
      catch (Exception exc)
      {
        HandleDeploymentException(exc, uniqueClientId);
      }
    }

    public void DeployAsync(Guid uniqueClientId, string requesterIdentity, DeploymentInfo deploymentInfo)
    {
      try
      {
        Guard.NotNull(deploymentInfo, "DeploymentInfo");
        Guard.NotNullNorEmpty(deploymentInfo.ProjectName, "DeploymentInfo.ProjectName");

        ProjectInfo projectInfo =
          _projectInfoRepository.FindByName(deploymentInfo.ProjectName);

        if (projectInfo == null)
        {
          throw new FaultException<ProjectNotFoundFault>(new ProjectNotFoundFault { ProjectName = deploymentInfo.ProjectName });
        }

        ThreadPool.QueueUserWorkItem(
          state =>
          {
            try
            {
              DoDeploy(uniqueClientId, requesterIdentity, deploymentInfo, projectInfo);
            }
            catch (Exception exc)
            {
              HandleDeploymentException(exc, uniqueClientId);
            }
          });
      }
      catch (Exception exc)
      {
        HandleDeploymentException(exc, uniqueClientId);
      }
    }

    public List<Proxy.Dto.ProjectInfo> GetProjectInfos(Proxy.Dto.ProjectFilter projectFilter)
    {
      if (projectFilter == null)
      {
        throw new ArgumentNullException("projectFilter");
      }

      IEnumerable<ProjectInfo> projectInfos =
        _projectInfoRepository.GetAll();

      if (!string.IsNullOrEmpty(projectFilter.Name))
      {
        projectInfos =
          projectInfos
            .Where(pi => !string.IsNullOrEmpty(pi.Name) && pi.Name.IndexOf(projectFilter.Name, StringComparison.CurrentCultureIgnoreCase) > -1);
      }

      return
        projectInfos
          .Select(DtoMapper.Map<ProjectInfo, Proxy.Dto.ProjectInfo>)
          .ToList();
    }

    public List<Proxy.Dto.EnvironmentInfo> GetEnvironmentInfos()
    {
      IEnumerable<EnvironmentInfo> environmentInfos =
        _environmentInfoRepository.GetAll();

      return
        environmentInfos
          .Select(DtoMapper.Map<EnvironmentInfo, Proxy.Dto.EnvironmentInfo>)
          .ToList();
    }

    public List<string> GetWebMachineNames(string environmentName)
    {
      if (string.IsNullOrEmpty(environmentName))
      {
        throw new ArgumentException("Environment name can't be null or empty", "environmentName");
      }

      EnvironmentInfo environmentInfo = _environmentInfoRepository.FindByName(environmentName);

      if (environmentInfo == null)
      {
        throw new FaultException<EnvironmentNotFoundFault>(
          new EnvironmentNotFoundFault
          {
            EnvironmentName = environmentName
          });
      }

      return environmentInfo.WebServerMachineNames.ToList();
    }

    public List<ProjectConfiguration> GetProjectConfigurations(string projectName, Proxy.Dto.ProjectConfigurationFilter projectConfigurationFilter)
    {
      if (string.IsNullOrEmpty(projectName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectName");
      }

      if (projectConfigurationFilter == null)
      {
        throw new ArgumentNullException("projectConfigurationFilter");
      }

      ProjectInfo projectInfo =
        _projectInfoRepository.FindByName(projectName);

      Core.TeamCity.Models.Project project =
        projectInfo != null
          ? _teamCityClient.GetProjectByName(projectInfo.ArtifactsRepositoryName)
          : null;

      Core.TeamCity.Models.ProjectDetails projectDetails =
        project != null
          ? _teamCityClient.GetProjectDetails(project)
          : null;

      if (projectDetails == null)
      {
        throw new FaultException<ProjectNotFoundFault>(new ProjectNotFoundFault { ProjectName = projectName });
      }

      if (projectDetails.ConfigurationsList == null || projectDetails.ConfigurationsList.Configurations == null)
      {
        return new List<ProjectConfiguration>();
      }

      IEnumerable<Core.TeamCity.Models.ProjectConfiguration> projectConfigurations =
        projectDetails.ConfigurationsList.Configurations;

      if (!string.IsNullOrEmpty(projectConfigurationFilter.Name))
      {
        projectConfigurations =
          projectConfigurations
            .Where(pc => !string.IsNullOrEmpty(pc.Name) && pc.Name.IndexOf(projectConfigurationFilter.Name, StringComparison.CurrentCultureIgnoreCase) > -1);
      }

      return projectConfigurations
        .Select(DtoMapper.Map<Core.TeamCity.Models.ProjectConfiguration, ProjectConfiguration>)
        .ToList();
    }

    public List<ProjectConfigurationBuild> GetProjectConfigurationBuilds(string projectName, string projectConfigurationName, int maxCount, Proxy.Dto.ProjectConfigurationBuildFilter projectConfigurationBuildFilter)
    {
      if (string.IsNullOrEmpty(projectName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectName");
      }

      if (string.IsNullOrEmpty(projectConfigurationName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectConfigurationName");
      }

      if (projectConfigurationBuildFilter == null)
      {
        throw new ArgumentNullException("projectConfigurationBuildFilter");
      }

      ProjectInfo projectInfo =
        _projectInfoRepository.FindByName(projectName);

      Core.TeamCity.Models.Project project =
        projectInfo != null
          ? _teamCityClient.GetProjectByName(projectInfo.ArtifactsRepositoryName)
          : null;

      Core.TeamCity.Models.ProjectDetails projectDetails =
        project != null
          ? _teamCityClient.GetProjectDetails(project)
          : null;

      if (projectDetails == null)
      {
        throw new FaultException<ProjectNotFoundFault>(new ProjectNotFoundFault { ProjectName = projectName });
      }

      Core.TeamCity.Models.ProjectConfiguration projectConfiguration =
        (projectDetails.ConfigurationsList != null && projectDetails.ConfigurationsList.Configurations != null)
          ? projectDetails.ConfigurationsList.Configurations
              .SingleOrDefault(pc => pc.Name == projectConfigurationName)
          : null;

      Core.TeamCity.Models.ProjectConfigurationDetails projectConfigurationDetails =
        projectConfiguration != null
          ? _teamCityClient.GetProjectConfigurationDetails(projectConfiguration)
          : null;

      if (projectConfigurationDetails == null)
      {
        throw new FaultException<ProjectConfigurationNotFoundFault>(new ProjectConfigurationNotFoundFault { ProjectName = projectInfo.Name, ProjectConfigurationName = projectConfigurationName });
      }

      Core.TeamCity.Models.ProjectConfigurationBuildsList projectConfigurationBuildsList =
        _teamCityClient.GetProjectConfigurationBuilds(projectConfigurationDetails, 0, maxCount);

      if (projectConfigurationBuildsList.Builds == null)
      {
        return new List<ProjectConfigurationBuild>();
      }

      IEnumerable<Core.TeamCity.Models.ProjectConfigurationBuild> projectConfigurationBuilds =
        projectConfigurationBuildsList.Builds;

      if (!string.IsNullOrEmpty(projectConfigurationBuildFilter.Number))
      {
        projectConfigurationBuilds =
          projectConfigurationBuilds
            .Where(pcb => !string.IsNullOrEmpty(pcb.Number) && pcb.Number.IndexOf(projectConfigurationBuildFilter.Number, StringComparison.CurrentCultureIgnoreCase) > -1);
      }

      return projectConfigurationBuilds
        .Select(DtoMapper.Map<Core.TeamCity.Models.ProjectConfigurationBuild, ProjectConfigurationBuild>)
        .ToList();
    }

    public List<string> GetWebAppProjectTargetUrls(string projectName, string environmentName)
    {
      if (string.IsNullOrEmpty(projectName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectName");
      }

      if (string.IsNullOrEmpty(environmentName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "environmentName");
      }

      WebAppProjectInfo webAppProjectInfo =
        _projectInfoRepository.FindByName(projectName) as WebAppProjectInfo;

      if (webAppProjectInfo == null)
      {
        throw new FaultException<ProjectNotFoundFault>(new ProjectNotFoundFault { ProjectName = projectName });
      }

      EnvironmentInfo environmentInfo =
        _environmentInfoRepository.FindByName(environmentName);

      if (environmentInfo == null)
      {
        throw new FaultException<EnvironmentNotFoundFault>(new EnvironmentNotFoundFault { EnvironmentName = environmentName });
      }

      List<string> targetUrls =
        webAppProjectInfo.GetTargetUrls(environmentInfo)
          .ToList();

      return targetUrls;
    }

    public List<string> GetProjectTargetFolders(string projectName, string environmentName)
    {
      if (string.IsNullOrEmpty(projectName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "projectName");
      }

      if (string.IsNullOrEmpty(environmentName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "environmentName");
      }

      ProjectInfo projectInfo =
        _projectInfoRepository.FindByName(projectName);

      if (projectInfo == null)
      {
        throw new FaultException<ProjectNotFoundFault>(new ProjectNotFoundFault { ProjectName = projectName });
      }

      EnvironmentInfo environmentInfo =
        _environmentInfoRepository.FindByName(environmentName);

      if (environmentInfo == null)
      {
        throw new FaultException<EnvironmentNotFoundFault>(new EnvironmentNotFoundFault { EnvironmentName = environmentName });
      }

      List<string> targetFolders =
        projectInfo.GetTargetFolders(ObjectFactory.Instance, environmentInfo)
          .ToList();

      return targetFolders;
    }

    public List<Proxy.Dto.DeploymentRequest> GetDeploymentRequests(int startIndex, int maxCount)
    {
      return
        _deploymentRequestRepository.GetDeploymentRequests(startIndex, maxCount)
          .Select(DtoMapper.Map<DeploymentRequest, Proxy.Dto.DeploymentRequest>)
          .ToList();
    }

    public List<Proxy.Dto.DiagnosticMessage> GetDiagnosticMessages(Guid uniqueClientId, long lastSeenMaxMessageId, Proxy.Dto.DiagnosticMessageType minMessageType)
    {
      if (uniqueClientId == Guid.Empty)
      {
        throw new ArgumentException("Argument can't be Guid.Empty.", "uniqueClientId");
      }

      return
        _diagnosticMessagesLogger.GetMessages(uniqueClientId, lastSeenMaxMessageId)
          .Select(DtoMapper.Map<DiagnosticMessage, Proxy.Dto.DiagnosticMessage>)
          .Where(dm => dm.Type >= minMessageType)
          .ToList();
    }

    public Proxy.Dto.Metadata.ProjectMetadata GetProjectMetadata(string projectName, string environmentName)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");
      Guard.NotNullNorEmpty(environmentName, "environmentName");

      try
      {
        ProjectMetadata projectMetadata =
          _projectMetadataExplorer.GetProjectMetadata(projectName, environmentName);

        return
          new Proxy.Dto.Metadata.ProjectMetadata
          {
            ProjectName = projectMetadata.ProjectName,
            EnvironmentName = projectMetadata.EnvironmentName,
            ProjectVersions =
              projectMetadata.ProjectVersions
                .Select(
                  pv =>
                  new MachineSpecificProjectVersion
                  {
                    MachineName = pv.MachineName,
                    ProjectVersion = pv.ProjectVersion,
                  }).ToList(),
          };
      }
      catch (Exception exc)
      {
        _log.ErrorIfEnabled(() => "Unhandled exception.", exc);
        
        throw;
      }
    }

    #endregion

    #region Private methods

    private void HandleDeploymentException(Exception exception, Guid uniqueClientId)
    {
      const string errorMessage = "Unhandled exception.";

      _diagnosticMessagesLogger
        .LogMessage(
          uniqueClientId,
          DiagnosticMessageType.Error,
          string.Format("{0}{1}", errorMessage, (exception != null ? Environment.NewLine + exception : " (no exception info)")));

      _log.ErrorIfEnabled(() => errorMessage, exception);
    }

    private void DoDeploy(Guid uniqueClientId, string requesterIdentity, DeploymentInfo deploymentInfoDto, ProjectInfo projectInfo)
    {
      Core.Domain.DeploymentInfo deploymentInfo =
        DtoMapper.ConvertDeploymentInfo(deploymentInfoDto, projectInfo);

      DeploymentTask deploymentTask =
        projectInfo.CreateDeploymentTask(ObjectFactory.Instance);

      var deploymentContext =
        new DeploymentContext(requesterIdentity);

      EventHandler<DiagnosticMessageEventArgs> deploymentPipelineDiagnosticMessageAction =
        (eventSender, tmpArgs) =>
        {
          _log.DebugIfEnabled(() => string.Format("{0}: {1}", tmpArgs.MessageType, tmpArgs.Message));

          _diagnosticMessagesLogger.LogMessage(uniqueClientId, tmpArgs.MessageType, tmpArgs.Message);
        };

      try
      {
        _deploymentPipeline.DiagnosticMessagePosted += deploymentPipelineDiagnosticMessageAction;

        _deploymentPipeline.StartDeployment(deploymentInfo, deploymentTask, deploymentContext);
      }
      finally
      {
        _deploymentPipeline.DiagnosticMessagePosted -= deploymentPipelineDiagnosticMessageAction;
      }
    }

    #endregion
  }
}
