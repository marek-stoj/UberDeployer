using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain.InputParameters;

namespace UberDeployer.Core.Domain
{
  public class DeploymentInfo
  {
    public DeploymentInfo(
      string projectName,
      string projectConfigurationName,
      string projectConfigurationBuildId,
      string targetEnvironmentName,      
      ProjectInfo projectInfo,
      InputParams inputParams)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");
      Guard.NotNullNorEmpty(projectConfigurationName, "projectConfigurationName");
      Guard.NotNullNorEmpty(projectConfigurationBuildId, "projectConfigurationBuildId");
      Guard.NotNullNorEmpty(targetEnvironmentName, "targetEnvironmentName");
      Guard.NotNull(projectInfo, "projectInfo");
      Guard.NotNull(inputParams, "inputParams");

      ProjectName = projectName;
      ProjectConfigurationName = projectConfigurationName;
      ProjectConfigurationBuildId = projectConfigurationBuildId;
      TargetEnvironmentName = targetEnvironmentName;
      ProjectInfo = projectInfo;
      InputParams = inputParams;
    }

    public string ProjectName { get; private set; }
    public string ProjectConfigurationName { get; private set; }
    public string ProjectConfigurationBuildId { get; private set; }
    public string TargetEnvironmentName { get; private set; }    
    public ProjectInfo ProjectInfo { get; private set; }
    public InputParams InputParams { get; private set; }
  }
}
