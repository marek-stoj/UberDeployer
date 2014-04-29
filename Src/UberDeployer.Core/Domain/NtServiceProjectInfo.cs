using System.Collections.Generic;
using System.IO;
using System.Linq;
using UberDeployer.Common.SyntaxSugar;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain.Input;
using UberDeployer.Core.Management.FailoverCluster;

namespace UberDeployer.Core.Domain
{
  public class NtServiceProjectInfo : ProjectInfo
  {
    public NtServiceProjectInfo(string name, string artifactsRepositoryName, IEnumerable<string> allowedEnvironmentNames, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string ntServiceName, string ntServiceDirName, string ntServiceDisplayName, string ntServiceExeName, string ntServiceUserId)
      : base(name, artifactsRepositoryName, allowedEnvironmentNames, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
      Guard.NotNullNorEmpty(ntServiceName, "ntServiceName");
      Guard.NotNullNorEmpty(ntServiceDirName, "ntServiceDirName");
      Guard.NotNullNorEmpty(ntServiceExeName, "ntServiceExeName");
      Guard.NotNullNorEmpty(ntServiceUserId, "ntServiceUserId");

      NtServiceName = ntServiceName;
      NtServiceDisplayName = ntServiceDisplayName;
      NtServiceDirName = ntServiceDirName;
      NtServiceExeName = ntServiceExeName;
      NtServiceUserId = ntServiceUserId;
    }

    public override ProjectType Type
    {
      get { return ProjectType.NtService; }
    }

    public override InputParams CreateEmptyInputParams()
    {
      return new NtServiceInputParams();
    }

    public override DeploymentTask CreateDeploymentTask(IObjectFactory objectFactory)
    {
      return
        new DeployNtServiceDeploymentTask(
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

    public override IEnumerable<string> GetTargetFolders(IObjectFactory objectFactory, EnvironmentInfo environmentInfo)
    {
      Guard.NotNull(objectFactory, "objectFactory");
      Guard.NotNull(environmentInfo, "environmentInfo");

      if (environmentInfo.EnableFailoverClusteringForNtServices)
      {
        IFailoverClusterManager failoverClusterManager =
          objectFactory.CreateFailoverClusterManager();

        string clusterGroupName =
          environmentInfo.GetFailoverClusterGroupNameForProject(Name);

        IEnumerable<string> possibleNodeNames =
          failoverClusterManager.GetPossibleNodeNames(
            environmentInfo.FailoverClusterMachineName,
            clusterGroupName);

        return
          possibleNodeNames
            .Select(node => EnvironmentInfo.GetNetworkPath(node, Path.Combine(environmentInfo.NtServicesBaseDirPath, NtServiceDirName)))
            .ToList();
      }
      else
      {
        return
          new List<string>
          {
            environmentInfo.GetAppServerNetworkPath(
              Path.Combine(environmentInfo.NtServicesBaseDirPath, NtServiceDirName))
          };
      }
    }

    public override string GetMainAssemblyFileName()
    {
      return NtServiceExeName;
    }

    public string NtServiceName { get; private set; }

    public string NtServiceDirName { get; private set; }

    public string NtServiceDisplayName { get; private set; }

    public string NtServiceExeName { get; private set; }

    /// <summary>
    /// A reference to a user that will be used to run the scheduled task. Users are defined in target environments.
    /// </summary>
    public string NtServiceUserId { get; private set; }
  }
}
