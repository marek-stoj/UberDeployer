using System;
using System.Collections.Generic;
using UberDeployer.Agent.Proxy.Dto;
using UberDeployer.Agent.Proxy.Dto.Metadata;
using UberDeployer.Agent.Proxy.Dto.TeamCity;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.Agent.Proxy
{
  public class AgentServiceClient : WcfProxy<IAgentService>, IAgentService
  {
    #region IAgentService Members

    public void Deploy(Guid uniqueClientId, string requesterIdentity, DeploymentInfo deploymentInfo)
    {
      Exec(@as => @as.Deploy(uniqueClientId, requesterIdentity, deploymentInfo));
    }

    public void DeployAsync(Guid uniqueClientId, string requesterIdentity, DeploymentInfo deploymentInfo)
    {
      Exec(@as => @as.DeployAsync(uniqueClientId, requesterIdentity, deploymentInfo));
    }

    public List<ProjectInfo> GetProjectInfos(ProjectFilter projectFilter)
    {
      return Exec(@as => @as.GetProjectInfos(projectFilter));
    }

    public List<EnvironmentInfo> GetEnvironmentInfos()
    {
      return Exec(@as => @as.GetEnvironmentInfos());
    }

    public List<ProjectConfiguration> GetProjectConfigurations(string projectName, ProjectConfigurationFilter projectConfigurationFilter)
    {
      return Exec(@as => @as.GetProjectConfigurations(projectName, projectConfigurationFilter));
    }

    public List<ProjectConfigurationBuild> GetProjectConfigurationBuilds(string projectName, string projectConfigurationName, int maxCount, ProjectConfigurationBuildFilter projectConfigurationBuildFilter)
    {
      return Exec(@as => @as.GetProjectConfigurationBuilds(projectName, projectConfigurationName, maxCount, projectConfigurationBuildFilter));
    }

    public List<string> GetWebAppProjectTargetUrls(string projectName, string environmentName)
    {
      return Exec(@as => @as.GetWebAppProjectTargetUrls(projectName, environmentName));
    }

    public List<string> GetProjectTargetFolders(string projectName, string environmentName)
    {
      return Exec(@as => @as.GetProjectTargetFolders(projectName, environmentName));
    }

    public List<DeploymentRequest> GetDeploymentRequests(int startIndex, int maxCount)
    {
      return Exec(@as => @as.GetDeploymentRequests(startIndex, maxCount));
    }

    public List<DiagnosticMessage> GetDiagnosticMessages(Guid uniqueClientId, long lastSeenMaxMessageId, DiagnosticMessageType minMessageType)
    {
      return Exec(@as => @as.GetDiagnosticMessages(uniqueClientId, lastSeenMaxMessageId, minMessageType));
    }

    public List<string> GetWebMachineNames(string environmentName)
    {
      return Exec(@as => @as.GetWebMachineNames(environmentName));
    }

    public ProjectMetadata GetProjectMetadata(string projectName, string environmentName)
    {
      return Exec(@as => @as.GetProjectMetadata(projectName, environmentName));
    }

    #endregion
  }
}
