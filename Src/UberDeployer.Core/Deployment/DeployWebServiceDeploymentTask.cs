using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Iis;
using UberDeployer.Core.Management.MsDeploy;

namespace UberDeployer.Core.Deployment
{
  public class DeployWebServiceDeploymentTask : DeployWebAppDeploymentTask
  {
    #region Constructor(s)

    public DeployWebServiceDeploymentTask(IMsDeploy msDeploy,IEnvironmentInfoRepository environmentInfoRepository, IArtifactsRepository artifactsRepository, IIisManager iisManager, WebAppProjectInfo projectInfo, string projectConfigurationName, string projectConfigurationBuildId, string targetEnvironmentName)
      : base(msDeploy, environmentInfoRepository, artifactsRepository, iisManager, projectInfo, projectConfigurationName, projectConfigurationBuildId, targetEnvironmentName)
    {
    }

    #endregion

    #region Overrides of DeployWebAppDeploymentTask

    public override string Description
    {
      get
      {
        return
          string.Format(
            "Deploy web service '{0} ({1}:{2})' to '{3}'.",
            _projectInfo.Name,
            _projectConfigurationName,
            _projectConfigurationBuildId,
            _targetEnvironmentName);
      }
    }

    #endregion
  }
}
