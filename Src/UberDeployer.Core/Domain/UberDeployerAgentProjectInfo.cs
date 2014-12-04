using System.Collections.Generic;
using UberDeployer.Core.Deployment;

namespace UberDeployer.Core.Domain
{
  public class UberDeployerAgentProjectInfo : NtServiceProjectInfo
  {
    #region Constructor(s)

    public UberDeployerAgentProjectInfo(string name, string artifactsRepositoryName, IEnumerable<string> allowedEnvironmentNames, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string ntServiceName, string ntServiceDirName, string ntServiceDisplayName, string ntServiceExeName, string ntServiceUserId)
      : base(name, artifactsRepositoryName, allowedEnvironmentNames, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific, ntServiceName, ntServiceDirName, ntServiceDisplayName, ntServiceExeName, ntServiceUserId, string.Empty)
    {
    }

    #endregion

    #region Overrides of ProjectInfo

    public override ProjectType Type
    {
      get { return ProjectType.UberDeployerAgent; }
    }

    public override DeploymentTask CreateDeploymentTask(IObjectFactory objectFactory)
    {
      return
        new DeployUberDeployerAgentDeploymentTask(
          objectFactory.CreateProjectInfoRepository(),
          objectFactory.CreateEnvironmentInfoRepository(),
          objectFactory.CreateArtifactsRepository(),
          objectFactory.CreateNtServiceManager(),
          objectFactory.CreatePasswordCollector(),
          objectFactory.CreateFailoverClusterManager(),
          objectFactory.CreateDirectoryAdapter(),
          objectFactory.CreateFileAdapter(),
          objectFactory.CreateZipFileAdapter());
    }

    #endregion
  }
}
