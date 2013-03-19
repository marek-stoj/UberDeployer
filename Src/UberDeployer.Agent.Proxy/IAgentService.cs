using System;
using System.Collections.Generic;
using System.ServiceModel;
using UberDeployer.Agent.Proxy.Dto;
using UberDeployer.Agent.Proxy.Dto.Metadata;
using UberDeployer.Agent.Proxy.Dto.TeamCity;
using UberDeployer.Agent.Proxy.Faults;

namespace UberDeployer.Agent.Proxy
{
  [ServiceContract]
  public interface IAgentService
  {
    [OperationContract]
    [FaultContract(typeof(ProjectNotFoundFault))]
    void Deploy(Guid uniqueClientId, string requesterIdentity, DeploymentInfo deploymentInfo);

    [OperationContract]
    [FaultContract(typeof(ProjectNotFoundFault))]
    void DeployAsync(Guid uniqueClientId, string requesterIdentity, DeploymentInfo deploymentInfo);

    [OperationContract]
    List<ProjectInfo> GetProjectInfos(ProjectFilter projectFilter);

    [OperationContract]
    List<EnvironmentInfo> GetEnvironmentInfos();    

    [OperationContract]
    [FaultContract(typeof(EnvironmentNotFoundFault))]
    List<string> GetWebMachineNames(string environmentName);

    [OperationContract]
    [FaultContract(typeof(ProjectNotFoundFault))]
    List<ProjectConfiguration> GetProjectConfigurations(string projectName, ProjectConfigurationFilter projectConfigurationFilter);

    [OperationContract]
    [FaultContract(typeof(ProjectNotFoundFault))]
    [FaultContract(typeof(ProjectConfigurationNotFoundFault))]
    List<ProjectConfigurationBuild> GetProjectConfigurationBuilds(string projectName, string projectConfigurationName, int maxCount, ProjectConfigurationBuildFilter projectConfigurationBuildFilter);

    [OperationContract]
    [FaultContract(typeof(ProjectNotFoundFault))]
    [FaultContract(typeof(EnvironmentNotFoundFault))]
    List<string> GetWebAppProjectTargetUrls(string projectName, string environmentName);

    [OperationContract]
    [FaultContract(typeof(ProjectNotFoundFault))]
    [FaultContract(typeof(EnvironmentNotFoundFault))]
    List<string> GetProjectTargetFolders(string projectName, string environmentName);

    // TODO IMM HI: separate interface?
    [OperationContract]
    List<DeploymentRequest> GetDeploymentRequests(int startIndex, int maxCount);

    // TODO IMM HI: separate interface?
    [OperationContract]
    List<DiagnosticMessage> GetDiagnosticMessages(Guid uniqueClientId, long lastSeenMaxMessageId, DiagnosticMessageType minMessageType);

    // TODO IMM HI: separate interface?
    [OperationContract]
    ProjectMetadata GetProjectMetadata(string projectName, string environmentName);
  }
}
