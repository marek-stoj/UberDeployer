using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Deployment;

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

    public string ProjectName { get; set; }
    public string ProjectConfigurationName { get; set; }
    public string ProjectConfigurationBuildId { get; set; }
    public string TargetEnvironmentName { get; set; }    
    public ProjectInfo ProjectInfo { get; set; }
    public InputParams InputParams { get; set; }
  }
}
