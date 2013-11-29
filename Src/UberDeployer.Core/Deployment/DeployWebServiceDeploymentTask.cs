using UberDeployer.Common.IO;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Management.Iis;
using UberDeployer.Core.Management.MsDeploy;

namespace UberDeployer.Core.Deployment
{
  public class DeployWebServiceDeploymentTask : DeployWebAppDeploymentTask
  {
    #region Constructor(s)

    public DeployWebServiceDeploymentTask(IProjectInfoRepository projectInfoRepository, IEnvironmentInfoRepository environmentInfoRepository, IMsDeploy msDeploy, IArtifactsRepository artifactsRepository, IIisManager iisManager, IFileAdapter fileAdapter, IZipFileAdapter zipFileAdapter)
      : base(projectInfoRepository, environmentInfoRepository, msDeploy, artifactsRepository, iisManager, fileAdapter, zipFileAdapter)
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
            DeploymentInfo.ProjectName,
            DeploymentInfo.ProjectConfigurationName,
            DeploymentInfo.ProjectConfigurationBuildId,
            DeploymentInfo.TargetEnvironmentName);
      }
    }

    #endregion
  }
}
