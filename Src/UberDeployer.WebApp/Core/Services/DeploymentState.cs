using System;
using UberDeployer.Common.SyntaxSugar;

namespace UberDeployer.WebApp.Core.Services
{
  public class DeploymentState
  {
    public DeploymentState(Guid deploymentId, string userIdentity, string projectName, string projectConfigurationName, string projectConfigurationBuildId, string targetEnvironmentName)
    {
      Guard.NotEmpty(deploymentId, "deploymentId");
      Guard.NotNullNorEmpty(userIdentity, "userIdentity");
      Guard.NotNullNorEmpty(projectName, "projectName");
      Guard.NotNullNorEmpty(projectConfigurationName, "projectConfigurationName");
      Guard.NotNullNorEmpty(projectConfigurationBuildId, "projectConfigurationBuildId");
      Guard.NotNullNorEmpty(targetEnvironmentName, "targetEnvironmentName");

      DeploymentId = deploymentId;
      UserIdentity = userIdentity;
      ProjectName = projectName;
      ProjectConfigurationName = projectConfigurationName;
      ProjectConfigurationBuildId = projectConfigurationBuildId;
      TargetEnvironmentName = targetEnvironmentName;
    }

    public Guid DeploymentId { get; private set; }

    public string UserIdentity { get; private set; }
    
    public string ProjectName { get; private set; }
    
    public string ProjectConfigurationName { get; private set; }
    
    public string ProjectConfigurationBuildId { get; private set; }

    public string TargetEnvironmentName { get; private set; }
  }
}
