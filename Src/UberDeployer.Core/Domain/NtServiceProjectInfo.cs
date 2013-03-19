using System;
using System.Collections.Generic;
using System.IO;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain.Input;

namespace UberDeployer.Core.Domain
{
  public class NtServiceProjectInfo : ProjectInfo
  {
    #region Constructor(s)

    public NtServiceProjectInfo(string name, string artifactsRepositoryName, string artifactsRepositoryDirName, bool artifactsAreNotEnvironmentSpecific, string ntServiceName, string ntServiceDirName, string ntServiceDisplayName, string ntServiceExeName, string ntServiceUserId)
      : base(name, artifactsRepositoryName, artifactsRepositoryDirName, artifactsAreNotEnvironmentSpecific)
    {
      if (string.IsNullOrEmpty(ntServiceName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "ntServiceName");
      }

      if (string.IsNullOrEmpty(ntServiceDirName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "ntServiceDirName");
      }

      if (string.IsNullOrEmpty(ntServiceExeName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "ntServiceExeName");
      }

      if (string.IsNullOrEmpty(ntServiceUserId))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "ntServiceUserId");
      }

      NtServiceName = ntServiceName;
      NtServiceDisplayName = ntServiceDisplayName;
      NtServiceDirName = ntServiceDirName;
      NtServiceExeName = ntServiceExeName;
      NtServiceUserId = ntServiceUserId;
    }

    #endregion

    #region Overrides of ProjectInfo

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
      if (objectFactory == null)
      {
        throw new ArgumentNullException("objectFactory");
      }

      return
        new DeployNtServiceDeploymentTask(
          objectFactory.CreateEnvironmentInfoRepository(),
          objectFactory.CreateArtifactsRepository(),
          objectFactory.CreateNtServiceManager(),
          objectFactory.CreatePasswordCollector(),
          objectFactory.CreateFailoverClusterManager());
    }

    public override IEnumerable<string> GetTargetFolders(EnvironmentInfo environmentInfo)
    {
      if (environmentInfo == null)
      {
        throw new ArgumentNullException("environmentInfo");
      }

      // TODO IMM HI: what about clustered environment?
      return
        new List<string>
          {
            environmentInfo.GetAppServerNetworkPath(
              Path.Combine(environmentInfo.NtServicesBaseDirPath, NtServiceDirName))
          };
    }

    public override string GetMainAssemblyFileName()
    {
      return NtServiceExeName;
    }

    #endregion

    #region Properties

    public string NtServiceName { get; private set; }

    public string NtServiceDirName { get; private set; }

    public string NtServiceDisplayName { get; private set; }

    public string NtServiceExeName { get; private set; }

    /// <summary>
    /// A reference to a user that will be used to run the scheduled task. Users are defined in target environments.
    /// </summary>
    public string NtServiceUserId { get; private set; }

    #endregion
  }
}
