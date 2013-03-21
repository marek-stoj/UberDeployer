using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Domain.Input;

namespace UberDeployer.Core.Domain
{
  public class DeploymentInfo
  {
    public DeploymentInfo(
      bool isSimulation,
      string projectName,
      string projectConfigurationName,
      string projectConfigurationBuildId,
      string targetEnvironmentName,
      InputParams inputParams)
    {
      Guard.NotNullNorEmpty(projectName, "projectName");
      Guard.NotNullNorEmpty(projectConfigurationName, "projectConfigurationName");
      Guard.NotNullNorEmpty(projectConfigurationBuildId, "projectConfigurationBuildId");
      Guard.NotNullNorEmpty(targetEnvironmentName, "targetEnvironmentName");
      Guard.NotNull(inputParams, "inputParams");

      IsSimulation = isSimulation;
      ProjectName = projectName;
      ProjectConfigurationName = projectConfigurationName;
      ProjectConfigurationBuildId = projectConfigurationBuildId;
      TargetEnvironmentName = targetEnvironmentName;
      InputParams = inputParams;
    }

    public bool IsSimulation { get; private set; }

    public string ProjectName { get; private set; }

    public string ProjectConfigurationName { get; private set; }

    public string ProjectConfigurationBuildId { get; private set; }

    public string TargetEnvironmentName { get; private set; }    

    public InputParams InputParams { get; private set; }
  }
}
